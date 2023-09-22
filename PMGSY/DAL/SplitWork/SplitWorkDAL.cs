/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: SplitWorkDAL.cs

 * Author : Koustubh Nakate

 * Creation Date :02/July/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of split work screens.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using System.Data.Entity;
using PMGSY.Models.SplitWork;
using System.Text;
using System.Transactions;
using PMGSY.Extensions;
using System.Data.Entity.Core;
namespace PMGSY.DAL.SplitWork
{
    public class SplitWorkDAL : ISplitWorkDAL
    {

        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        /// <summary>
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        private Int64 GetMaxCode()
        {
            Int64? maxCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                maxCode = (from IMSWorks in dbContext.IMS_PROPOSAL_WORK select (Int64?)IMSWorks.IMS_WORK_CODE).Max();

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

        public Array GetSplitWorkDetailsListDAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool isAgreementDone = false;
            bool isAlreadySplit = false;

            bool isFinalize = false;

            try
            {

                var query = from IMSWorks in dbContext.IMS_PROPOSAL_WORK
                            join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                            on IMSWorks.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                            where
                            IMSWorks.IMS_PR_ROAD_CODE == IMSPRRoadCode
                            select new
                            {
                                IMSWorks.IMS_WORK_CODE,
                                IMSWorks.IMS_PR_ROAD_CODE,
                                IMSWorks.IMS_WORK_DESC,
                                IMSWorks.IMS_PAV_LENGTH,
                                IMSWorks.IMS_START_CHAINAGE,
                                IMSWorks.IMS_END_CHAINAGE,
                                IMSWorks.IMS_PAV_EST_COST,
                                IMSWorks.IMS_CD_WORKS_EST_COST,
                                IMSWorks.IMS_PROTECTION_WORKS,
                                IMSWorks.IMS_OTHER_WORK_COST,
                                IMSWorks.IMS_STATE_SHARE,
                                IMSWorks.IMS_BRIDGE_WORKS_EST_COST,
                                IMSWorks.IMS_BRIDGE_EST_COST_STATE,
                                IMSWorks.IMS_MAINTENANCE_YEAR1,
                                IMSWorks.IMS_MAINTENANCE_YEAR2,
                                IMSWorks.IMS_MAINTENANCE_YEAR3,
                                IMSWorks.IMS_MAINTENANCE_YEAR4,
                                IMSWorks.IMS_MAINTENANCE_YEAR5,
                                IMSWorks.IMS_MAINTENANCE_YEAR6


                            };

                isAgreementDone = dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS == "P");

                isFinalize = dbContext.IMS_PROPOSAL_SPLIT.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.IMS_SPLIT_STATUS == "Y");

                isAlreadySplit = query.Count() > 0 ? true : false;

                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "WorkName":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StartChainage":
                                query = query.OrderBy(x => x.IMS_START_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderBy(x => x.IMS_END_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PevementLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PevementCost":
                                query = query.OrderBy(x => x.IMS_PAV_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "CDWorksCost":
                                query = query.OrderBy(x => x.IMS_CD_WORKS_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "ProtectionCost":
                                query = query.OrderBy(x => x.IMS_PROTECTION_WORKS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "OtherWorksCost":
                                query = query.OrderBy(x => x.IMS_OTHER_WORK_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateShare":
                                query = query.OrderBy(x => x.IMS_STATE_SHARE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "WorkName":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StartChainage":
                                query = query.OrderByDescending(x => x.IMS_START_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderByDescending(x => x.IMS_END_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PevementLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "PevementCost":
                                query = query.OrderByDescending(x => x.IMS_PAV_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "CDWorksCost":
                                query = query.OrderByDescending(x => x.IMS_CD_WORKS_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "ProtectionCost":
                                query = query.OrderByDescending(x => x.IMS_PROTECTION_WORKS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "OtherWorksCost":
                                query = query.OrderByDescending(x => x.IMS_OTHER_WORK_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateShare":
                                query = query.OrderByDescending(x => x.IMS_STATE_SHARE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSWorks => new
                {

                    IMSWorks.IMS_WORK_CODE,
                    IMSWorks.IMS_PR_ROAD_CODE,
                    IMSWorks.IMS_WORK_DESC,
                    IMSWorks.IMS_PAV_LENGTH,
                    IMSWorks.IMS_START_CHAINAGE,
                    IMSWorks.IMS_END_CHAINAGE,
                    IMSWorks.IMS_PAV_EST_COST,
                    IMSWorks.IMS_CD_WORKS_EST_COST,
                    IMSWorks.IMS_PROTECTION_WORKS,
                    IMSWorks.IMS_OTHER_WORK_COST,
                    IMSWorks.IMS_STATE_SHARE,
                    IMSWorks.IMS_BRIDGE_WORKS_EST_COST,
                    IMSWorks.IMS_BRIDGE_EST_COST_STATE,
                    IMSWorks.IMS_MAINTENANCE_YEAR1,
                    IMSWorks.IMS_MAINTENANCE_YEAR2,
                    IMSWorks.IMS_MAINTENANCE_YEAR3,
                    IMSWorks.IMS_MAINTENANCE_YEAR4,
                    IMSWorks.IMS_MAINTENANCE_YEAR5,
                    IMS_MAINTENANCE_YEAR6 = IMSWorks.IMS_MAINTENANCE_YEAR6.HasValue ? IMSWorks.IMS_MAINTENANCE_YEAR6.Value : 0,

                }).ToArray();


                return result.Select(IMSWorks => new
                {
                    //id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                                                               
                                    IMSWorks.IMS_WORK_DESC.ToString(),
                                    IMSWorks.IMS_START_CHAINAGE.ToString(),
                                    IMSWorks.IMS_END_CHAINAGE.ToString(),
                                    IMSWorks.IMS_PAV_LENGTH.ToString(),
                                    IMSWorks.IMS_PAV_EST_COST.ToString(),
                                    IMSWorks.IMS_CD_WORKS_EST_COST.ToString(),
                                    IMSWorks.IMS_PROTECTION_WORKS.ToString(),
                                    IMSWorks.IMS_OTHER_WORK_COST.ToString(),
                                    IMSWorks.IMS_STATE_SHARE.ToString(),   
                                    PMGSYSession.Current.PMGSYScheme == 1 ? ((IMSWorks.IMS_MAINTENANCE_YEAR1)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR2)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR3)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR4)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR5)
                                    ).ToString()
                                    :
                                    ((IMSWorks.IMS_MAINTENANCE_YEAR1)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR2)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR3)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR4)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR5)+
                                       (IMSWorks.IMS_MAINTENANCE_YEAR6)
                                    ).ToString(),
                                   isAgreementDone==true?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSWorks.IMS_PR_ROAD_CODE.ToString(),"IMSWorkCode =" + IMSWorks.IMS_WORK_CODE.ToString()  }),
                                   (isAgreementDone==true|| isFinalize==true )?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSWorks.IMS_PR_ROAD_CODE.ToString(),"IMSWorkCode =" + IMSWorks.IMS_WORK_CODE.ToString()})
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


        public bool SaveSplitWorkDetailsDAL(SplitWorkDetails splitWorkDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                IMS_PROPOSAL_WORK imsProposalWork = null;
                encryptedParameters = splitWorkDetails.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());


                if (dbContext.IMS_PROPOSAL_SPLIT.Any(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode && ps.IMS_SPLIT_STATUS == "Y"))
                {
                    message = "You can not split work, because already split work has been finalized.";
                    return false;
                }

                if (dbContext.IMS_PROPOSAL_SPLIT.Any(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode) && (dbContext.IMS_PROPOSAL_WORK.Count(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode) == dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault()))
                {

                    message = "You can not split work , because already split count exceeds its limit.";
                    return false;
                }

                if (dbContext.IMS_PROPOSAL_WORK.Any(pw => pw.IMS_WORK_DESC.ToUpper() == splitWorkDetails.IMS_WORK_DESC.ToUpper()))
                {
                    message = "Work Name is already exist.";
                    return false;
                }

                if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS == "P"))
                {
                    message = "You can not split work, because already agreement has been done on selected work.";
                    return false;
                }

                if (!CheckAllCost(dbContext, IMSPRRoadCode, 0, splitWorkDetails, ref message))
                {
                    return false;
                }

                //  if (!dbContext.IMS_PROPOSAL_SPLIT.Any(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                //    {
                if (!CheckMaintenanceCost(dbContext, 0, IMSPRRoadCode, splitWorkDetails, ref message))
                {
                    return false;
                }
                //  }

                imsProposalWork = new IMS_PROPOSAL_WORK();

                imsProposalWork.IMS_WORK_CODE = (Int32)GetMaxCode();
                imsProposalWork.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                imsProposalWork.IMS_WORK_DESC = splitWorkDetails.IMS_WORK_DESC;
                imsProposalWork.IMS_PAV_LENGTH = ((Decimal)splitWorkDetails.IMS_END_CHAINAGE - (Decimal)splitWorkDetails.IMS_START_CHAINAGE);   //(Decimal)splitWorkDetails.IMS_PAV_LENGTH;
                imsProposalWork.IMS_START_CHAINAGE = (Decimal)splitWorkDetails.IMS_START_CHAINAGE;
                imsProposalWork.IMS_END_CHAINAGE = (Decimal)splitWorkDetails.IMS_END_CHAINAGE;
                imsProposalWork.IMS_PAV_EST_COST = (Decimal)splitWorkDetails.IMS_PAV_EST_COST;
                imsProposalWork.IMS_CD_WORKS_EST_COST = (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST;
                imsProposalWork.IMS_PROTECTION_WORKS = (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS;
                imsProposalWork.IMS_OTHER_WORK_COST = (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST;
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    imsProposalWork.IMS_STATE_SHARE = (Decimal)splitWorkDetails.IMS_STATE_SHARE;
                }
                ///Changes for RCPLWE
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    //short sharePercent = Convert.ToInt16(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(m => m.IMS_SHARE_PERCENT).FirstOrDefault());
                    //if (sharePercent == 1)
                    //{
                    //    imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.10);
                    //}
                    //else if (sharePercent == 2)
                    //{
                    //    imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.25);
                    //}

                    ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                    short sharePercent = Convert.ToInt16(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(m => m.IMS_SHARE_PERCENT_2015).FirstOrDefault());
                    if (sharePercent == 1)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.25);
                    }
                    else if (sharePercent == 2)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.10);
                    }
                    else if (sharePercent == 3)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST +  (splitWorkDetails.IMS_FURNITURE_COST.HasValue ? (Decimal)splitWorkDetails.IMS_FURNITURE_COST : 0) + (splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST.HasValue ? (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST : 0)) * Convert.ToDecimal(0.40);
                    }
                    else if (sharePercent == 4)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.0);
                    }
                }

                if (splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST != null)
                {
                    //imsProposalWork.IMS_BRIDGE_WORKS_EST_COST = splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST == null ? 0 : ((decimal)splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST);
                    imsProposalWork.IMS_BRIDGE_WORKS_EST_COST = ((decimal)splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST);
                }
                if (splitWorkDetails.IMS_BRIDGE_EST_COST_STATE != null)
                {
                    //imsProposalWork.IMS_BRIDGE_EST_COST_STATE = splitWorkDetails.IMS_BRIDGE_EST_COST_STATE == null ? 0 : ((decimal)splitWorkDetails.IMS_BRIDGE_EST_COST_STATE);
                    imsProposalWork.IMS_BRIDGE_EST_COST_STATE = ((decimal)splitWorkDetails.IMS_BRIDGE_EST_COST_STATE);
                }

                imsProposalWork.IMS_MAINTENANCE_YEAR1 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR1;
                imsProposalWork.IMS_MAINTENANCE_YEAR2 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR2;
                imsProposalWork.IMS_MAINTENANCE_YEAR3 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR3;
                imsProposalWork.IMS_MAINTENANCE_YEAR4 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR4;
                imsProposalWork.IMS_MAINTENANCE_YEAR5 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR5;
                
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    imsProposalWork.IMS_MAINTENANCE_YEAR6 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR6;
                    imsProposalWork.IMS_HIGHER_SPECIFICATION_COST = (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST;
                    imsProposalWork.IMS_FURNITURE_COST = (Decimal)splitWorkDetails.IMS_FURNITURE_COST;
                }

                ///Changes for RCPLWE
                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    imsProposalWork.IMS_MAINTENANCE_YEAR6 = splitWorkDetails.IMS_MAINTENANCE_YEAR6.HasValue ? (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR6 : 0;
                }
                //added by abhishek kamble 27-nov-2013
                imsProposalWork.USERID = PMGSYSession.Current.UserId;
                imsProposalWork.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.IMS_PROPOSAL_WORK.Add(imsProposalWork);
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
        }//end SaveSplitWorkDetailsDAL()

        private bool CheckMaintenanceCost(PMGSYEntities dbContext, int IMSWorkCode, int IMSPRRoadCode, SplitWorkDetails splitWorkDetails, ref string message)
        {
            bool result = true;
            decimal sanctionedMaintenanceCost = 0;
            decimal? maintenanceCost = 0;
            decimal? existingMaintenanceCost = 0;
            List<IMS_PROPOSAL_WORK> splitWorkDetailsList = null;

            IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

            if (IMSWorkCode == 0)
            {

                splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList<IMS_PROPOSAL_WORK>();
            }
            else
            {
                //dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && (IMSWorkCode == 0 ? 1 : pw.IMS_WORK_CODE) != (IMSWorkCode == 0 ? 1 : IMSWorkCode));
                splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE != IMSWorkCode).ToList<IMS_PROPOSAL_WORK>();
            }

            if (splitWorkDetailsList.Count() > 0)
            {
                decimal? mainCostYear1 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1));

                decimal? mainCostYear2 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2));

                decimal? mainCostYear3 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3));

                decimal? mainCostYear4 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4));

                decimal? mainCostYear5 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5));

                decimal? mainCostYear6 = null;
                ///Changes for RCPLWE
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    mainCostYear6 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6));
                }


                existingMaintenanceCost = (mainCostYear1 + mainCostYear2 + mainCostYear3 + mainCostYear4 + mainCostYear5);
                ///Changes for RCPLWE
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    existingMaintenanceCost = existingMaintenanceCost + mainCostYear6;
                }


            }

            if (IMSSanctioned != null)
            {
                sanctionedMaintenanceCost = (IMSSanctioned.IMS_SANCTIONED_MAN_AMT1 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT2 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT3 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT4 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT5);
                ///Changes for RCPLWE
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    sanctionedMaintenanceCost += IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT.Value;
                }


                if (sanctionedMaintenanceCost != 0 && sanctionedMaintenanceCost != existingMaintenanceCost)
                {
                    maintenanceCost = (splitWorkDetails.IMS_MAINTENANCE_YEAR1 + splitWorkDetails.IMS_MAINTENANCE_YEAR2 + splitWorkDetails.IMS_MAINTENANCE_YEAR3 + splitWorkDetails.IMS_MAINTENANCE_YEAR4 + splitWorkDetails.IMS_MAINTENANCE_YEAR5);
                    ///Changes for RCPLWE
                    if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                    {
                        maintenanceCost += splitWorkDetails.IMS_MAINTENANCE_YEAR6.HasValue ? splitWorkDetails.IMS_MAINTENANCE_YEAR6.Value : 0;
                    }

                    maintenanceCost = maintenanceCost == null ? 0 : maintenanceCost;

                    if (maintenanceCost == 0)
                    {
                        message = "Sanctioned maintenace cost is greater than 0, so enter maintenance cost.";
                        return false;
                    }
                }
            }

            return result;

        }

        private bool CheckAllCost(PMGSYEntities dbContext, int IMSPRRoadCode, int IMSWorkCode, SplitWorkDetails splitWorkDetails, ref string message)
        {
            bool result = true;
            StringBuilder strMessage = new StringBuilder("<ul>");

            IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

            List<IMS_PROPOSAL_WORK> splitWorkDetailsList = null;
            try
            {
                if (IMSWorkCode == 0)
                {

                    splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList<IMS_PROPOSAL_WORK>();
                }
                else
                {
                    //dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && (IMSWorkCode == 0 ? 1 : pw.IMS_WORK_CODE) != (IMSWorkCode == 0 ? 1 : IMSWorkCode));
                    splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE != IMSWorkCode).ToList<IMS_PROPOSAL_WORK>();
                }

                if (IMSSanctioned != null)
                {
                    //decimal? pavLength = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_LENGTH) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_LENGTH)) + ((Decimal)splitWorkDetails.IMS_END_CHAINAGE - (Decimal)splitWorkDetails.IMS_START_CHAINAGE);

                    //if (pavLength > IMSSanctioned.IMS_PAV_LENGTH)
                    //{
                    //    result = false;
                    //    strMessage.Append("<li>Total pavement length exceeds sanctioned pavement length.</li>");
                    //}

                    decimal? pavLength = ((Decimal)splitWorkDetails.IMS_END_CHAINAGE - (Decimal)splitWorkDetails.IMS_START_CHAINAGE);

                    if (pavLength > IMSSanctioned.IMS_PAV_LENGTH)
                    {
                        result = false;
                        strMessage.Append("<li>Chainage length [" + String.Format("{0:0.000#}", pavLength) + "] exceeds sanctioned pavement length [" + IMSSanctioned.IMS_PAV_LENGTH + "].</li>");
                    }

                    decimal? pavCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_EST_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_EST_COST)) + splitWorkDetails.IMS_PAV_EST_COST;

                    if (pavCost > IMSSanctioned.IMS_SANCTIONED_PAV_AMT)
                    {
                        result = false;
                        strMessage.Append("<li>Total pavement cost [" + String.Format("{0:0.000#}", pavCost) + "] exceeds sanctioned pavement cost [" + IMSSanctioned.IMS_SANCTIONED_PAV_AMT + "].</li>");
                    }

                    decimal? CDWorksCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_CD_WORKS_EST_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_CD_WORKS_EST_COST)) + splitWorkDetails.IMS_CD_WORKS_EST_COST;

                    if (CDWorksCost > IMSSanctioned.IMS_SANCTIONED_CD_AMT)
                    {
                        result = false;
                        strMessage.Append("<li>Total CD works cost [" + String.Format("{0:0.000#}", CDWorksCost) + "] exceeds sanctioned CD works cost ["+ IMSSanctioned.IMS_SANCTIONED_CD_AMT +"].</li>");
                    }

                    decimal? protectionCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PROTECTION_WORKS) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PROTECTION_WORKS)) + splitWorkDetails.IMS_PROTECTION_WORKS;

                    if (protectionCost > IMSSanctioned.IMS_SANCTIONED_PW_AMT)
                    {
                        result = false;
                        //strMessage.Append("<li>Total protection cost exceeds sanctioned protection cost.</li>");
                        strMessage.Append("<li>Total protection cost [" + String.Format("{0:0.000#}", protectionCost.Value) + "] exceeds sanctioned protection cost [" + IMSSanctioned.IMS_SANCTIONED_PW_AMT + "].</li>");
                    }

                    decimal? otherWorksCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_OTHER_WORK_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_OTHER_WORK_COST)) + splitWorkDetails.IMS_OTHER_WORK_COST;

                    if (otherWorksCost > IMSSanctioned.IMS_SANCTIONED_OW_AMT)
                    {
                        result = false;
                        strMessage.Append("<li>Total other works cost [" + String.Format("{0:0.000#}", otherWorksCost) + "] exceeds sanctioned other works cost [" + IMSSanctioned.IMS_SANCTIONED_OW_AMT + "].</li>");
                    }

                    decimal? stateShare = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_STATE_SHARE) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_STATE_SHARE)) + splitWorkDetails.IMS_STATE_SHARE;

                    //if (stateShare > IMSSanctioned.IMS_SANCTIONED_RS_AMT)

                    decimal? totstateShare = PMGSYSession.Current.PMGSYScheme == 1 ? (IMSSanctioned.IMS_SANCTIONED_RS_AMT + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015)) : (Convert.ToDecimal(IMSSanctioned.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) + IMSSanctioned.IMS_SANCTIONED_BS_AMT);

                    ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                    if (PMGSYSession.Current.PMGSYScheme == 1 ? (stateShare > (IMSSanctioned.IMS_SANCTIONED_RS_AMT + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015))) : (stateShare > (Convert.ToDecimal(IMSSanctioned.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) + IMSSanctioned.IMS_SANCTIONED_BS_AMT)))
                    {
                        result = false;
                        strMessage.Append("<li>Total state share cost [" + String.Format("{0:0.000#}", stateShare) + "] exceeds sanctioned state share cost [" + totstateShare + "].</li>");
                    }

                    decimal? furnitureCost = null;
                    decimal? higherSpecCost = null;
                    ///NA for RCPLWE
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        furnitureCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_FURNITURE_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_FURNITURE_COST)) + splitWorkDetails.IMS_FURNITURE_COST;
                        higherSpecCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_HIGHER_SPECIFICATION_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_HIGHER_SPECIFICATION_COST)) + splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST;

                        if (furnitureCost > IMSSanctioned.IMS_SANCTIONED_FC_AMT.Value)
                        {
                            result = false;
                            strMessage.Append("<li>Total Furniture cost [" + String.Format("{0:0.000#}", furnitureCost) + "] exceeds sanctioned Furniture cost [" + IMSSanctioned.IMS_SANCTIONED_FC_AMT.Value + "].</li>");
                        }

                        if (higherSpecCost > (IMSSanctioned.IMS_HIGHER_SPECIFICATION_COST.HasValue ? IMSSanctioned.IMS_SANCTIONED_HS_AMT.Value : 0))
                        {
                            decimal? tothigherSpecCost = (IMSSanctioned.IMS_HIGHER_SPECIFICATION_COST.HasValue ? IMSSanctioned.IMS_SANCTIONED_HS_AMT.Value : 0);
                            result = false;
                            strMessage.Append("<li>Total Higher Specification cost [" + String.Format("{0:0.000#}", higherSpecCost) + "] exceeds Sanctioned Higher Specification Cost [" + tothigherSpecCost + "].</li>");
                        }
                    }



                    //decimal? bridgeWork = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_BRIDGE_WORKS_EST_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_BRIDGE_WORKS_EST_COST)) + (splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST == null ? 0 : splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST);

                    //if (bridgeWork > IMSSanctioned.IMS_SANCTIONED_BW_AMT)
                    //{
                    //    result = false;
                    //    strMessage.Append("<li>Total bridge work cost exceeds sanctioned bridge work cost.</li>");
                    //}

                    //decimal? bridgeStateShare = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_BRIDGE_EST_COST_STATE) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_BRIDGE_EST_COST_STATE)) + (splitWorkDetails.IMS_BRIDGE_EST_COST_STATE == null ? 0 : splitWorkDetails.IMS_BRIDGE_EST_COST_STATE);

                    //if (bridgeStateShare > IMSSanctioned.IMS_SANCTIONED_BS_AMT)
                    //{
                    //    result = false;
                    //    strMessage.Append("<li>Total bridge state share cost exceeds sanctioned bridge state share cost.</li>");
                    //}

                    #region Validation commented as per equirement from Shrinivasa Sir directions on 13 October, 2016
                    /*
                decimal? mainCostYear1 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1)) + splitWorkDetails.IMS_MAINTENANCE_YEAR1;

                if (mainCostYear1 > IMSSanctioned.IMS_SANCTIONED_MAN_AMT1)
                {
                    result = false;
                    strMessage.Append("<li>Total maintenance year1 cost exceeds sanctioned maintenance year1 cost.</li>");
                }

                decimal? mainCostYear2 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2)) + splitWorkDetails.IMS_MAINTENANCE_YEAR2;

                if (mainCostYear2 > IMSSanctioned.IMS_SANCTIONED_MAN_AMT2)
                {
                    result = false;
                    strMessage.Append("<li>Total maintenance year2 cost exceeds sanctioned maintenance year2 cost.</li>");
                }

                decimal? mainCostYear3 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3)) + splitWorkDetails.IMS_MAINTENANCE_YEAR3;

                if (mainCostYear3 > IMSSanctioned.IMS_SANCTIONED_MAN_AMT3)
                {
                    result = false;
                    strMessage.Append("<li>Total maintenance year3 cost exceeds sanctioned maintenance year3 cost.</li>");
                }

                decimal? mainCostYear4 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4)) + splitWorkDetails.IMS_MAINTENANCE_YEAR4;

                if (mainCostYear4 > IMSSanctioned.IMS_SANCTIONED_MAN_AMT4)
                {
                    result = false;
                    strMessage.Append("<li>Total maintenance year4 cost exceeds sanctioned maintenance year4 cost.</li>");
                }

                decimal? mainCostYear5 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5)) + splitWorkDetails.IMS_MAINTENANCE_YEAR5;

                if (mainCostYear5 > IMSSanctioned.IMS_SANCTIONED_MAN_AMT5)
                {
                    result = false;
                    strMessage.Append("<li>Total maintenance year5 cost exceeds sanctioned maintenance year5 cost.</li>");
                }
                */
                    #endregion
                    decimal? mainCostYear6 = null;
                    ///Changes for RCPLWE
                    if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                    {
                        mainCostYear6 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6)) + splitWorkDetails.IMS_MAINTENANCE_YEAR6;
                        if (mainCostYear6 > IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT)
                        {
                            result = false;
                            strMessage.Append("<li>Total renewal cost year6 [" + String.Format("{0:0.000#}", mainCostYear6) + "] exceeds sanctioned renewal cost year6 [" + IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT + "].</li>");
                        }
                    }

                }

                if (!result)
                {
                    strMessage.Append("</ul>");
                    message = strMessage.ToString();
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CheckAllCost()");
                return false;
            }
        }


        public SplitWorkDetails GetSplitWorkDetailsDAL(int IMSPRRoadCode, int IMSWorkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions commonFunction = new CommonFunctions();

                IMS_PROPOSAL_WORK IMSProposalWork = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE == IMSWorkCode).FirstOrDefault();

                SplitWorkDetails splitWorkDetails = null;


                if (IMSProposalWork != null)
                {

                    splitWorkDetails = new SplitWorkDetails()
                    {

                        EncryptedIMSPRRoadCode = URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSProposalWork.IMS_PR_ROAD_CODE.ToString(), "IMSWorkCode =" + IMSProposalWork.IMS_WORK_CODE.ToString() }),
                        EncryptedIMSWorkCode = URLEncrypt.EncryptParameters1(new string[] { "IMSWorkCode =" + IMSProposalWork.IMS_WORK_CODE.ToString() }),
                        IMS_WORK_DESC = IMSProposalWork.IMS_WORK_DESC.Trim(),
                        IMS_START_CHAINAGE = IMSProposalWork.IMS_START_CHAINAGE,
                        IMS_END_CHAINAGE = IMSProposalWork.IMS_END_CHAINAGE,
                        IMS_PAV_LENGTH = IMSProposalWork.IMS_PAV_LENGTH,
                        IMS_PAV_EST_COST = IMSProposalWork.IMS_PAV_EST_COST,
                        IMS_CD_WORKS_EST_COST = IMSProposalWork.IMS_CD_WORKS_EST_COST,
                        IMS_PROTECTION_WORKS = IMSProposalWork.IMS_PROTECTION_WORKS,
                        IMS_OTHER_WORK_COST = IMSProposalWork.IMS_OTHER_WORK_COST,
                        IMS_STATE_SHARE = IMSProposalWork.IMS_STATE_SHARE,
                        IMS_BRIDGE_WORKS_EST_COST = IMSProposalWork.IMS_BRIDGE_WORKS_EST_COST,
                        IMS_BRIDGE_EST_COST_STATE = IMSProposalWork.IMS_BRIDGE_EST_COST_STATE,
                        IMS_MAINTENANCE_YEAR1 = IMSProposalWork.IMS_MAINTENANCE_YEAR1,
                        IMS_MAINTENANCE_YEAR2 = IMSProposalWork.IMS_MAINTENANCE_YEAR2,
                        IMS_MAINTENANCE_YEAR3 = IMSProposalWork.IMS_MAINTENANCE_YEAR3,
                        IMS_MAINTENANCE_YEAR4 = IMSProposalWork.IMS_MAINTENANCE_YEAR4,
                        IMS_MAINTENANCE_YEAR5 = IMSProposalWork.IMS_MAINTENANCE_YEAR5,
                        SanctionedCostDetails = GetSanctionedCostDetailsDAL(IMSPRRoadCode, IMSWorkCode)

                    };

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        splitWorkDetails.IMS_FURNITURE_COST = IMSProposalWork.IMS_FURNITURE_COST;
                        splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST = IMSProposalWork.IMS_HIGHER_SPECIFICATION_COST;
                        splitWorkDetails.IMS_MAINTENANCE_YEAR6 = IMSProposalWork.IMS_MAINTENANCE_YEAR6;
                        //splitWorkDetails.SharePercent = (Int16)dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSProposalWork.IMS_PR_ROAD_CODE).Select(m => m.IMS_SHARE_PERCENT).FirstOrDefault();
                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        splitWorkDetails.SharePercent = (Int16)dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSProposalWork.IMS_PR_ROAD_CODE).Select(m => m.IMS_SHARE_PERCENT_2015).FirstOrDefault();
                    }
                    if (PMGSYSession.Current.PMGSYScheme == 3)
                    {
                        splitWorkDetails.IMS_MAINTENANCE_YEAR6 = IMSProposalWork.IMS_MAINTENANCE_YEAR6;
                    }
                }

                return splitWorkDetails;

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetSplitWorkDetailsDAL()");
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

        public bool UpdateSplitWorkDetailsDAL(SplitWorkDetails splitWorkDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                int IMSWorkCode = 0;
                IMS_PROPOSAL_WORK imsProposalWork = null;
                encryptedParameters = splitWorkDetails.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());
                IMSWorkCode = Convert.ToInt32(decryptedParameters["IMSWorkCode"].ToString().Trim());



                if (dbContext.IMS_PROPOSAL_WORK.Any(pw => pw.IMS_WORK_DESC.ToUpper() == splitWorkDetails.IMS_WORK_DESC.ToUpper() && pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE != IMSWorkCode))
                {
                    message = "Work Name is already exist.";
                    return false;
                }

                if (!CheckAllCost(dbContext, IMSPRRoadCode, IMSWorkCode, splitWorkDetails, ref message))
                {
                    return false;
                }

                if (!CheckMaintenanceCost(dbContext, IMSWorkCode, IMSPRRoadCode, splitWorkDetails, ref message))
                {
                    return false;
                }

                imsProposalWork = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE == IMSWorkCode).FirstOrDefault();

                if (imsProposalWork == null)
                {
                    return false;
                }

                imsProposalWork.IMS_WORK_DESC = splitWorkDetails.IMS_WORK_DESC;
                imsProposalWork.IMS_PAV_LENGTH = ((Decimal)splitWorkDetails.IMS_END_CHAINAGE - (Decimal)splitWorkDetails.IMS_START_CHAINAGE);//(Decimal)splitWorkDetails.IMS_PAV_LENGTH;
                imsProposalWork.IMS_START_CHAINAGE = (Decimal)splitWorkDetails.IMS_START_CHAINAGE;
                imsProposalWork.IMS_END_CHAINAGE = (Decimal)splitWorkDetails.IMS_END_CHAINAGE;
                imsProposalWork.IMS_PAV_EST_COST = (Decimal)splitWorkDetails.IMS_PAV_EST_COST;
                imsProposalWork.IMS_CD_WORKS_EST_COST = (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST;
                imsProposalWork.IMS_PROTECTION_WORKS = (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS;
                imsProposalWork.IMS_OTHER_WORK_COST = (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST;
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    imsProposalWork.IMS_STATE_SHARE = (Decimal)splitWorkDetails.IMS_STATE_SHARE;
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    short sharePercent = Convert.ToInt16(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(m => m.IMS_SHARE_PERCENT).FirstOrDefault());
                    if (sharePercent == 1)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.10);
                    }
                    else if (sharePercent == 2)
                    {
                        imsProposalWork.IMS_STATE_SHARE = ((Decimal)splitWorkDetails.IMS_PAV_EST_COST + (Decimal)splitWorkDetails.IMS_PROTECTION_WORKS + (Decimal)splitWorkDetails.IMS_OTHER_WORK_COST + (Decimal)splitWorkDetails.IMS_CD_WORKS_EST_COST + (Decimal)splitWorkDetails.IMS_FURNITURE_COST + (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST) * Convert.ToDecimal(0.25);
                    }
                }

                if (splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST != null)
                {
                    imsProposalWork.IMS_BRIDGE_WORKS_EST_COST = ((decimal)splitWorkDetails.IMS_BRIDGE_WORKS_EST_COST);
                }
                if (splitWorkDetails.IMS_BRIDGE_EST_COST_STATE != null)
                {
                    imsProposalWork.IMS_BRIDGE_EST_COST_STATE = ((decimal)splitWorkDetails.IMS_BRIDGE_EST_COST_STATE);
                }

                imsProposalWork.IMS_MAINTENANCE_YEAR1 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR1;
                imsProposalWork.IMS_MAINTENANCE_YEAR2 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR2;
                imsProposalWork.IMS_MAINTENANCE_YEAR3 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR3;
                imsProposalWork.IMS_MAINTENANCE_YEAR4 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR4;
                imsProposalWork.IMS_MAINTENANCE_YEAR5 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR5;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    imsProposalWork.IMS_MAINTENANCE_YEAR6 = (Decimal)splitWorkDetails.IMS_MAINTENANCE_YEAR6;
                    imsProposalWork.IMS_HIGHER_SPECIFICATION_COST = (Decimal)splitWorkDetails.IMS_HIGHER_SPECIFICATION_COST;
                    imsProposalWork.IMS_FURNITURE_COST = (Decimal)splitWorkDetails.IMS_FURNITURE_COST;
                }
                //added by abhishek kamble 27-nov-2013
                imsProposalWork.USERID = PMGSYSession.Current.UserId;
                imsProposalWork.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(imsProposalWork).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateSplitWorkDetailsDAL(OptimisticConcurrencyException ex)");
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateSplitWorkDetailsDAL(UpdateException ex)");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateSplitWorkDetailsDAL()");
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


        public bool DeleteSplitWorkDetailsDAL(int IMSPRRoadCode, int IMSWorkCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                using (var scope = new TransactionScope())
                {
                    IMS_PROPOSAL_WORK imsProposalWork = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE == IMSWorkCode).FirstOrDefault();

                    if (imsProposalWork == null)
                    {
                        return false;
                    }

                    //added by abhishek kamble 27-nov-2013
                    imsProposalWork.USERID = PMGSYSession.Current.UserId;
                    imsProposalWork.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(imsProposalWork).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.IMS_PROPOSAL_WORK.Remove(imsProposalWork);

                    //added by Koustubh Nakate on 11/11/2013 as integration Testing Round II dated 30 Sept 2013
                    if (dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count() == 1)
                    {
                        IMS_PROPOSAL_SPLIT imsProposalSplit = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

                        if (imsProposalSplit != null)
                        {
                            dbContext.IMS_PROPOSAL_SPLIT.Remove(imsProposalSplit);
                        }

                    }

                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteSplitWorkDetailsDAL(System.Data.Entity.Infrastructure.DbUpdateException ex)");
                message = "You can not delete this split work details.";
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteSplitWorkDetailsDAL()");
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


        public bool CheckAgreementExistBAL(int IMSPRRoadCode, ref bool isAgreementExist, ref bool isSplitWorkExist, ref bool isSplitCountExist)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                isAgreementExist = dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS == "P");

                isSplitWorkExist = dbContext.IMS_PROPOSAL_WORK.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode);

                isSplitCountExist = dbContext.IMS_PROPOSAL_SPLIT.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode);

                return true;

                //return dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode);
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


        public bool AddSplitCountBAL(SplitCount splitCount, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMSPRRoadCode = 0;
                IMS_PROPOSAL_SPLIT imsProposalSplit = null;
                encryptedParameters = splitCount.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString().Trim());

                if (dbContext.IMS_PROPOSAL_SPLIT.Any(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode))
                {
                    message = "Split Count for selected work is already exist.";
                    return false;
                }

                imsProposalSplit = new IMS_PROPOSAL_SPLIT();

                imsProposalSplit.IMS_PR_ROAD_CODE = IMSPRRoadCode;
                imsProposalSplit.IMS_TOTAL_SPLIT = (Int32)splitCount.IMS_TOTAL_SPLIT;
                imsProposalSplit.IMS_SPLIT_STATUS = "N";



                dbContext.IMS_PROPOSAL_SPLIT.Add(imsProposalSplit);
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "AddSplitCountDAL(OptimisticConcurrencyException ex)");
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "AddSplitCountDAL(UpdateException ex)");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "AddSplitCountDAL()");
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


        public string GetSanctionedCostDetailsDAL(int IMSPRRoadCode, int IMSWorkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //CommonFunctions commonFunction = new CommonFunctions();

                StringBuilder strMessage = new StringBuilder();

                IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

                List<IMS_PROPOSAL_WORK> splitWorkDetailsList = null;

                if (IMSWorkCode == 0)
                {

                    splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).ToList<IMS_PROPOSAL_WORK>();
                }
                else
                {
                    //dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && (IMSWorkCode == 0 ? 1 : pw.IMS_WORK_CODE) != (IMSWorkCode == 0 ? 1 : IMSWorkCode));
                    splitWorkDetailsList = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode && pw.IMS_WORK_CODE != IMSWorkCode).ToList<IMS_PROPOSAL_WORK>();
                }

                if (IMSSanctioned != null)
                {
                    strMessage.Append("<tr>");
                    strMessage.Append("<td style='color:#EB8F00;text-align:center'> " + "<b>Total</b>" + "</td>");
                    strMessage.Append("<td  style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_PAV_AMT.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_CD_AMT.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_PW_AMT.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_OW_AMT.ToString("F") + "</td>");


                    decimal? totalCost = null;

                    //change done by Vikram on 5 April 2014
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_FC_AMT.Value.ToString("F") + "</td>");
                        totalCost = IMSSanctioned.IMS_SANCTIONED_PAV_AMT + IMSSanctioned.IMS_SANCTIONED_CD_AMT + IMSSanctioned.IMS_SANCTIONED_PW_AMT + IMSSanctioned.IMS_SANCTIONED_OW_AMT + IMSSanctioned.IMS_SANCTIONED_FC_AMT;
                        if (IMSSanctioned.IMS_HIGHER_SPECIFICATION_COST != null)
                        {
                            strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_HS_AMT.Value.ToString("F") + "</td>");
                        }
                        else
                        {
                            strMessage.Append("<td style='text-align:center'> " + 0.ToString("F") + "</td>");
                        }
                        //if (IMSSanctioned.IMS_SHARE_PERCENT == 1)
                        //{
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.10)).ToString("F") + "</td>");
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.90)).ToString("F") + "</td>");
                        //}
                        //else if (IMSSanctioned.IMS_SHARE_PERCENT == 2)
                        //{
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.25)).ToString("F") + "</td>");
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.75)).ToString("F") + "</td>");
                        //}

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015).ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(IMSSanctioned.IMS_MORD_SHARE_2015).ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + (Convert.ToDecimal(IMSSanctioned.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) + IMSSanctioned.IMS_SANCTIONED_BS_AMT).ToString("F") + "</td>");
                    }
                    else
                    {
                        //strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_RS_AMT.ToString("F") + "</td>");

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_RS_AMT.ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(IMSSanctioned.IMS_MORD_SHARE_2015).ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_RS_AMT + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015)).ToString("F") + "</td>");
                    }
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_MAN_AMT1.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_MAN_AMT2.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_MAN_AMT3.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_MAN_AMT4.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_MAN_AMT5.ToString("F") + "</td>");
                    //change done by Vikram on 5 April 2014
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT.Value.ToString("F") + "</td>");
                    }
                    strMessage.Append("</tr>");


                    decimal? pavCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_EST_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PAV_EST_COST));
                    decimal? CDWorksCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_CD_WORKS_EST_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_CD_WORKS_EST_COST));
                    decimal? protectionCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PROTECTION_WORKS) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_PROTECTION_WORKS));
                    decimal? otherWorksCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_OTHER_WORK_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_OTHER_WORK_COST));
                    decimal? stateShare = null;
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        stateShare = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_STATE_SHARE) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_STATE_SHARE));
                    }


                    decimal? furnitureCost = null;
                    decimal? totalEnteredCost = null;
                    decimal? mordShare = null;
                    decimal? totMordShare = null;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        furnitureCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_FURNITURE_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_FURNITURE_COST));
                        totalEnteredCost = pavCost + CDWorksCost + protectionCost + otherWorksCost + furnitureCost;
                        //if (IMSSanctioned.IMS_SHARE_PERCENT == 1)
                        //{
                        //    stateShare = totalEnteredCost * Convert.ToDecimal(0.10);
                        //    mordShare = totalEnteredCost * Convert.ToDecimal(0.90);
                        //}
                        //else if (IMSSanctioned.IMS_SHARE_PERCENT == 2)
                        //{
                        //    stateShare = totalEnteredCost * Convert.ToDecimal(0.25);
                        //    mordShare = totalEnteredCost * Convert.ToDecimal(0.75);
                        //}

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        if (IMSSanctioned.IMS_SHARE_PERCENT_2015 == 1)
                        {
                            stateShare = totalEnteredCost * Convert.ToDecimal(0.25);
                            mordShare = totalEnteredCost * Convert.ToDecimal(0.75);
                        }
                        else if (IMSSanctioned.IMS_SHARE_PERCENT_2015 == 2)
                        {
                            stateShare = totalEnteredCost * Convert.ToDecimal(0.10);
                            mordShare = totalEnteredCost * Convert.ToDecimal(0.90);
                        }
                        else if (IMSSanctioned.IMS_SHARE_PERCENT_2015 == 3)
                        {
                            stateShare = totalEnteredCost * Convert.ToDecimal(0.40);
                            mordShare = totalEnteredCost * Convert.ToDecimal(0.60);
                        }
                        else if (IMSSanctioned.IMS_SHARE_PERCENT_2015 == 4)
                        {
                            stateShare = totalEnteredCost * Convert.ToDecimal(0);
                            mordShare = totalEnteredCost * Convert.ToDecimal(1);
                        }
                    }
                    decimal? higherSpecCost = null;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        higherSpecCost = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_HIGHER_SPECIFICATION_COST) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_HIGHER_SPECIFICATION_COST));
                    }
                    decimal? mainCostYear1 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR1));
                    decimal? mainCostYear2 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR2));
                    decimal? mainCostYear3 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR3));
                    decimal? mainCostYear4 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR4));
                    decimal? mainCostYear5 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR5));
                    decimal? mainCostYear6 = null;
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        mainCostYear6 = (splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6) == null ? 0 : splitWorkDetailsList.Sum(sp => (decimal?)sp.IMS_MAINTENANCE_YEAR6));
                    }


                    strMessage.Append("<tr >");
                    strMessage.Append("<td style='color:#EB8F00;text-align:center'> " + "<b>Split</b>" + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + pavCost.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + CDWorksCost.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + protectionCost.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + otherWorksCost.Value.ToString("F") + "</td>");


                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + furnitureCost.Value.ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + higherSpecCost.Value.ToString("F") + "</td>");
                        //strMessage.Append("<td style='text-align:center'> " + stateShare.Value.ToString("F") + "</td>");
                        //strMessage.Append("<td style='text-align:center'> " + mordShare.Value.ToString("F") + "</td>");

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        totMordShare = higherSpecCost.Value + stateShare.Value; /*+ IMSSanctioned.IMS_SANCTIONED_BS_AMT*/
                        strMessage.Append("<td style='text-align:center'> " + stateShare.Value.ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + mordShare.Value.ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + totMordShare.Value.ToString("F") + "</td>");
                    }
                    else
                    {
                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        strMessage.Append("<td style='text-align:center'> - </td>");
                        strMessage.Append("<td style='text-align:center'> - </td>");
                        strMessage.Append("<td style='text-align:center'> " + stateShare.Value.ToString("F") + "</td>");
                    }
                    strMessage.Append("<td style='text-align:center'> " + mainCostYear1.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + mainCostYear2.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + mainCostYear3.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + mainCostYear4.Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + mainCostYear5.Value.ToString("F") + "</td>");
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + mainCostYear6.Value.ToString("F") + "</td>");
                    }
                    strMessage.Append("</tr>");
                    strMessage.Append("<tr>");
                    strMessage.Append("<td style='color:#EB8F00;text-align:center'> " + "<b>Remaining Cost</b>" + "</td>");

                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_PAV_AMT - pavCost).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_CD_AMT - CDWorksCost).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_PW_AMT - protectionCost).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_OW_AMT - otherWorksCost).Value.ToString("F") + "</td>");

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_FC_AMT - furnitureCost).Value.ToString("F") + "</td>");
                        if (IMSSanctioned.IMS_HIGHER_SPECIFICATION_COST != null)
                        {
                            strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_HS_AMT - higherSpecCost).Value.ToString("F") + "</td>");
                        }
                        else
                        {
                            strMessage.Append("<td style='text-align:center'> " + 0.ToString("F") + "</td>");
                        }

                        //if (IMSSanctioned.IMS_SHARE_PERCENT == 1)
                        //{
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.10) - stateShare).ToString("F") + "</td>");
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.90) - mordShare).ToString("F") + "</td>");
                        //}
                        //else if (IMSSanctioned.IMS_SHARE_PERCENT == 2)
                        //{
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.25) - stateShare).ToString("F") + "</td>");
                        //    strMessage.Append("<td style='text-align:center'> " + Convert.ToDecimal(totalCost * Convert.ToDecimal(0.75) - mordShare).ToString("F") + "</td>");
                        //}

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        strMessage.Append("<td style='text-align:center'> " + (Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) - Convert.ToDecimal(stateShare)).ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + (Convert.ToDecimal(IMSSanctioned.IMS_MORD_SHARE_2015) - Convert.ToDecimal(mordShare)).ToString("F") + "</td>");
                        strMessage.Append("<td style='text-align:center'> " + ((Convert.ToDecimal(IMSSanctioned.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) + IMSSanctioned.IMS_SANCTIONED_BS_AMT) - Convert.ToDecimal(totMordShare)).ToString("F") + "</td>");
                    }
                    else
                    {
                        //strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_RS_AMT - stateShare).Value.ToString("F") + "</td>");

                        ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                        strMessage.Append("<td style='text-align:center'> - </td>");
                        strMessage.Append("<td style='text-align:center'> - </td>");
                        strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_RS_AMT + Convert.ToDecimal(IMSSanctioned.IMS_STATE_SHARE_2015) - Convert.ToDecimal(stateShare)).ToString("F") + "</td>");
                    }
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_MAN_AMT1 - mainCostYear1).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_MAN_AMT2 - mainCostYear2).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_MAN_AMT3 - mainCostYear3).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_MAN_AMT4 - mainCostYear4).Value.ToString("F") + "</td>");
                    strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_MAN_AMT5 - mainCostYear5).Value.ToString("F") + "</td>");
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        strMessage.Append("<td style='text-align:center'> " + (IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT - mainCostYear6).Value.ToString("F") + "</td>");
                    }
                    strMessage.Append("</tr>");
                }
                return strMessage.ToString();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetSanctionedCostDetailsDAL()");
                return string.Empty;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool FinalizedSplitWorkDetailsDAL(int IMSPRRoadCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int count = 0;

            try
            {
                IMS_PROPOSAL_SPLIT proposalSplit = null;

                count = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == IMSPRRoadCode).Count();

                proposalSplit = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

                if (proposalSplit == null)
                {
                    return false;
                }

                proposalSplit.IMS_SPLIT_STATUS = "Y";
                proposalSplit.IMS_TOTAL_SPLIT = count;
                dbContext.Entry(proposalSplit).State = System.Data.Entity.EntityState.Modified;
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

        public short? GetSharePercent(int imsprRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //return dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == imsprRoadCode).Select(m => m.IMS_SHARE_PERCENT).FirstOrDefault();
                
                ///Changed by SAMMED A. PATIL on 14JUNE2017 to display Total State Share Cost
                return dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == imsprRoadCode).Select(m => m.IMS_SHARE_PERCENT_2015).FirstOrDefault();
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


    }

    public interface ISplitWorkDAL
    {
        Array GetSplitWorkDetailsListDAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveSplitWorkDetailsDAL(SplitWorkDetails splitWorkDetails, ref string message);

        SplitWorkDetails GetSplitWorkDetailsDAL(int IMSPRRoadCode, int IMSWorkCode);

        bool UpdateSplitWorkDetailsDAL(SplitWorkDetails splitWorkDetails, ref string message);

        bool DeleteSplitWorkDetailsDAL(int IMSPRRoadCode, int IMSWorkCode, ref string message);

        bool CheckAgreementExistBAL(int IMSPRRoadCodebool, ref bool isAgreementExist, ref bool isSplitWorkExist, ref bool isSplitCountExist);

        bool AddSplitCountBAL(SplitCount splitCount, ref string message);

        string GetSanctionedCostDetailsDAL(int IMSPRRoadCode, int IMSWorkCode);

        bool FinalizedSplitWorkDetailsDAL(int IMSPRRoadCode, ref string message);
    }
}