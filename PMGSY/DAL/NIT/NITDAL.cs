/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: NITDAL.cs

 * Author : Koustubh Nakate

 * Creation Date :08/July/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of NIT screens.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using PMGSY.Models.NIT;
using System.Data.Entity;
using PMGSY.Extensions;
using System.Globalization;
using System.Transactions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.NIT
{
    public enum NITModules
    {
        NITMaster = 1,
        NITDetails
    };

    public class NITDAL:INITDAL
    {
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        /// <summary>
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        private Int64 GetMaxCode(NITModules NITModule)
        {
            Int64? maxCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                switch (NITModule)
                {
                    case NITModules.NITMaster:
                        maxCode = (from NITMaster in dbContext.TEND_NIT_MASTER select (Int64?)NITMaster.TEND_NIT_CODE).Max();
                        break;

                    case NITModules.NITDetails:
                        maxCode = (from NITDetail in dbContext.TEND_NIT_DETAILS select (Int64?)NITDetail.TEND_NIT_ID).Max();
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

        public Array GetNITDetailsDAL(int stateCode, int districtCode, int adminNDCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {         
            PMGSYEntities dbContext = new PMGSYEntities();
            try
           
            {


                var query = from NITMaster in dbContext.TEND_NIT_MASTER
                            join stateDetails in dbContext.MASTER_STATE
                            on NITMaster.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on NITMaster.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            join adminDetails in dbContext.ADMIN_DEPARTMENT
                            on NITMaster.ADMIN_ND_CODE equals adminDetails.ADMIN_ND_CODE
                            join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                            on NITMaster.TEND_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                            from fundingAgency in agencies.DefaultIfEmpty()
                            where
                            NITMaster.MAST_STATE_CODE == stateCode &&
                            NITMaster.MAST_DISTRICT_CODE == districtCode &&
                            NITMaster.ADMIN_ND_CODE == adminNDCode &&
                            NITMaster.TEND_LOCK_STATUS == "N" &&
                            NITMaster.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme // new change done by Vikram on 10 Feb 2014
                            select new
                            {
                                NITMaster.TEND_NIT_CODE,
                                NITMaster.TEND_NIT_NUMBER,
                                NITMaster.TEND_ISSUE_START_DATE,
                                NITMaster.TEND_ISSUE_END_DATE,
                                NITMaster.TEND_PUBLISH_TENDER,
                                NITMaster.TEND_LOCK_STATUS,
                                fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                NITMaster.TEND_ITEM_RATE,
                                NITMaster.TEND_PUBLICATION_DATE
                            };



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "NITNumber":
                                query = query.OrderBy(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IssueStartDate":
                                query = query.OrderBy(x => x.TEND_ISSUE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IssueEndDate":
                                query = query.OrderBy(x => x.TEND_ISSUE_END_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;                       
                            default:
                                query = query.OrderByDescending(x => x.TEND_NIT_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "NITNumber":
                                query = query.OrderByDescending(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IssueStartDate":
                                query = query.OrderByDescending(x => x.TEND_ISSUE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IssueEndDate":
                                query = query.OrderByDescending(x => x.TEND_ISSUE_END_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.TEND_NIT_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(NITMaster => new
                {
                    NITMaster.TEND_NIT_CODE,
                    NITMaster.TEND_NIT_NUMBER,
                    NITMaster.TEND_ISSUE_START_DATE,
                    NITMaster.TEND_ISSUE_END_DATE,
                    NITMaster.TEND_PUBLISH_TENDER,
                    NITMaster.TEND_LOCK_STATUS,
                    NITMaster.MAST_FUNDING_AGENCY_NAME,
                    NITMaster.TEND_ITEM_RATE,
                    NITMaster.TEND_PUBLICATION_DATE
                }).ToArray();


                return result.Select(NITMaster => new
                {
                    id = NITMaster.TEND_NIT_CODE.ToString(),
                    cell = new[] {                                                                               
                                    NITMaster.TEND_NIT_NUMBER,
                                    NITMaster.TEND_ISSUE_START_DATE==null?"NA":Convert.ToDateTime(NITMaster.TEND_ISSUE_START_DATE).ToString("dd/MM/yyyy") ,
                                    NITMaster.TEND_ISSUE_END_DATE==null?"NA":Convert.ToDateTime(NITMaster.TEND_ISSUE_END_DATE).ToString("dd/MM/yyyy") ,
                                    NITMaster.TEND_PUBLICATION_DATE == null?"NA":Convert.ToDateTime(NITMaster.TEND_PUBLICATION_DATE).ToString("dd/MM/yyyy") ,
                                    NITMaster.TEND_ITEM_RATE == null?"-":NITMaster.TEND_ITEM_RATE == "I"?"Item":"Percentage",
                                   (NITMaster.TEND_LOCK_STATUS=="Y")? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>":(NITMaster.TEND_PUBLISH_TENDER=="Y"?"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Road' onClick ='ViewNITRoads(\"" + URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + NITMaster.TEND_NIT_CODE.ToString(),"NITNumber =" + NITMaster.TEND_NIT_NUMBER.Replace("/","--").ToString(),"FundingAgency =" + NITMaster.MAST_FUNDING_AGENCY_NAME.ToString(),"TenderIssueStartDate=" + (NITMaster.TEND_ISSUE_START_DATE==null? string.Empty: Convert.ToDateTime(NITMaster.TEND_ISSUE_START_DATE).ToString("dd/MM/yyyy").Replace("/","--"))  }) + "\");'></span></td> </tr></table></center>" :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Road' onClick ='AddRoad(\"" + URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + NITMaster.TEND_NIT_CODE.ToString(),"NITNumber =" + NITMaster.TEND_NIT_NUMBER.Replace("/","--").ToString(),"FundingAgency =" + NITMaster.MAST_FUNDING_AGENCY_NAME.ToString(),"TenderIssueStartDate=" + (NITMaster.TEND_ISSUE_START_DATE==null? string.Empty: Convert.ToDateTime(NITMaster.TEND_ISSUE_START_DATE).ToString("dd/MM/yyyy").Replace("/","--"))  }) + "\");'></span></td> </tr></table></center>" )  ,//|| NITMaster.TEND_PUBLISH_TENDER=="Y"
                                    CheckNITRoadExist(NITMaster.TEND_NIT_CODE)==false ? ("<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='First add road details to publish NIT' ></span></td></tr></table></center>"):((NITMaster.TEND_LOCK_STATUS=="N" && NITMaster.TEND_PUBLISH_TENDER=="N") ?"<center><table><tr><td  style='border:none'><a href='#' title='Publish NIT' onClick ='PublishNIT(\"" + URLEncrypt.EncryptParameters1(new string[]{"TendNITCode="+NITMaster.TEND_NIT_CODE.ToString()}) + "\");' >Publish</a></td></tr></table></center>": "<center><table><tr><td style='border:none'>Published</td></tr></table></center>"),
                                    (NITMaster.TEND_LOCK_STATUS=="Y" || NITMaster.TEND_PUBLISH_TENDER=="Y")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + NITMaster.TEND_NIT_CODE.ToString()}),
                                   (NITMaster.TEND_LOCK_STATUS=="Y"  || NITMaster.TEND_PUBLISH_TENDER=="Y")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + NITMaster.TEND_NIT_CODE.ToString()})
                                  
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

        private bool CheckNITRoadExist(int tendNITCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                if (!dbContext.TEND_NIT_DETAILS.Any(NIT => NIT.TEND_NIT_CODE == tendNITCode))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public List<MASTER_FUNDING_AGENCY> GetFundingAgencies(bool isSearch)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<MASTER_FUNDING_AGENCY> fundingAgencyList = dbContext.MASTER_FUNDING_AGENCY.OrderBy(f => f.MAST_FUNDING_AGENCY_NAME).ToList<MASTER_FUNDING_AGENCY>();

                if (isSearch)
                {
                    fundingAgencyList.Insert(0, new MASTER_FUNDING_AGENCY() { MAST_FUNDING_AGENCY_CODE = 0, MAST_FUNDING_AGENCY_NAME = "All Agencies" });
                }
                else
                {
                    fundingAgencyList.Insert(0, new MASTER_FUNDING_AGENCY() { MAST_FUNDING_AGENCY_CODE = 0, MAST_FUNDING_AGENCY_NAME = "--Select--" });
                }

                return fundingAgencyList;

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


        public bool SaveNITDetailsDAL(NITDetails objNITDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;
                int districtCode = PMGSYSession.Current.DistrictCode;
                int adminNDCode = PMGSYSession.Current.AdminNdCode;
           
                TEND_NIT_MASTER tendNITMaster = null;
                CommonFunctions commonFunction = new CommonFunctions();

                if (dbContext.TEND_NIT_MASTER.Any(NIT => NIT.TEND_NIT_NUMBER.ToUpper() == objNITDetails.TEND_NIT_NUMBER.ToUpper()))
                {
                    message = "Draft NIT Number is already exist.";
                    return false;
                }


                tendNITMaster = new TEND_NIT_MASTER();

                tendNITMaster.TEND_NIT_CODE = (Int32)GetMaxCode(NITModules.NITMaster);
                tendNITMaster.MAST_STATE_CODE = stateCode;
                tendNITMaster.MAST_DISTRICT_CODE = districtCode;
                tendNITMaster.ADMIN_ND_CODE = adminNDCode;
                tendNITMaster.TEND_NIT_NUMBER = objNITDetails.TEND_NIT_NUMBER.Trim();

                //if (!string.IsNullOrEmpty(objNITDetails.TEND_ISSUE_START_TIME))
                //{
                //    objNITDetails.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE + " " + objNITDetails.TEND_ISSUE_START_TIME;
 
                //}
                //if (!string.IsNullOrEmpty(objNITDetails.TEND_ISSUE_END_TIME))
                //{
                //    objNITDetails.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE + " " + objNITDetails.TEND_ISSUE_END_TIME;
                //}
             
                //tendNITMaster.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.TEND_ISSUE_START_DATE);
                //tendNITMaster.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.TEND_ISSUE_END_DATE);

                tendNITMaster.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.TEND_ISSUE_START_DATE, objNITDetails.TEND_ISSUE_START_TIME);
                tendNITMaster.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.TEND_ISSUE_END_DATE, objNITDetails.TEND_ISSUE_END_TIME);
                
                
                tendNITMaster.TEND_INVITING_AUTHORITY = objNITDetails.TEND_INVITING_AUTHORITY == null ? null : objNITDetails.TEND_INVITING_AUTHORITY.Trim();
                tendNITMaster.TEND_INVITING_ORG = objNITDetails.TEND_INVITING_ORG == null ? null : objNITDetails.TEND_INVITING_ORG.Trim();
                tendNITMaster.CONT_REGN_WITH_ORG = objNITDetails.CONT_REGN_WITH_ORG == null ? null : objNITDetails.CONT_REGN_WITH_ORG.Trim();

                //if (!string.IsNullOrEmpty(objNITDetails.CONT_REGN_VALIDITY_TIME))
                //{
                //    objNITDetails.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE + " " + objNITDetails.CONT_REGN_VALIDITY_TIME;
                //}

               
                //tendNITMaster.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.CONT_REGN_VALIDITY_DATE);

                tendNITMaster.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.CONT_REGN_VALIDITY_DATE, objNITDetails.CONT_REGN_VALIDITY_TIME);

                tendNITMaster.TEND_DD_ISSUE_IN_FAVOUR = objNITDetails.TEND_DD_ISSUE_IN_FAVOUR == null ? null : objNITDetails.TEND_DD_ISSUE_IN_FAVOUR.Trim();
                tendNITMaster.TEND_DD_PAYABLE_AT = objNITDetails.TEND_DD_PAYABLE_AT == null ? null : objNITDetails.TEND_DD_PAYABLE_AT.Trim();

                if (objNITDetails.TEND_AMOUNT_PER_PACKAGE != null)
                {
                    tendNITMaster.TEND_AMOUNT_PER_PACKAGE = (Decimal)objNITDetails.TEND_AMOUNT_PER_PACKAGE; 
                }

                tendNITMaster.TEND_DOC_INSP_OFFICE = objNITDetails.TEND_DOC_INSP_OFFICE == null ? null : objNITDetails.TEND_DOC_INSP_OFFICE.Trim();
                tendNITMaster.TEND_INSP_START_DATE = objNITDetails.TEND_INSP_START_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_INSP_START_DATE);
                tendNITMaster.TEND_INSP_END_DATE = objNITDetails.TEND_INSP_END_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_INSP_END_DATE);
                tendNITMaster.TEND_PUBLICATION_DATE = objNITDetails.TEND_PUBLICATION_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_PUBLICATION_DATE);
                tendNITMaster.TEND_ITEM_RATE = objNITDetails.TendItemRate == true ? "I" : "P";
                tendNITMaster.TEND_PUBLISH_TENDER = "N";
                tendNITMaster.TEND_COLLABORATION = objNITDetails.TEND_COLLABORATION;
                tendNITMaster.TEND_LOCK_STATUS = "N";
                tendNITMaster.TEND_TENDER_TYPE = "T";

                //added by abhishek kamble 27-nov-2013
                tendNITMaster.USERID = PMGSYSession.Current.UserId;
                tendNITMaster.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                tendNITMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                dbContext.TEND_NIT_MASTER.Add(tendNITMaster);
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




        public NITDetails GetNITDetailsDAL(int tendNITCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions commonFunction = new CommonFunctions();

                TEND_NIT_MASTER tendNITMaster = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode && NIT.TEND_LOCK_STATUS == "N").FirstOrDefault();

                NITDetails objNITDetails = null;


                if (tendNITMaster != null)
                {

                    objNITDetails = new NITDetails()
                    {

                        EncryptedTendNITCode = URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + tendNITMaster.TEND_NIT_CODE.ToString() }),
                        TEND_COLLABORATION= (Int32)tendNITMaster.TEND_COLLABORATION,
                        TEND_NIT_NUMBER=tendNITMaster.TEND_NIT_NUMBER,
                        TEND_PUBLICATION_DATE = tendNITMaster.TEND_PUBLICATION_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_PUBLICATION_DATE).ToString("dd/MM/yyyy"),
                        TEND_ISSUE_START_DATE = tendNITMaster.TEND_ISSUE_START_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_ISSUE_START_DATE).ToString("dd/MM/yyyy"),
                        TEND_ISSUE_START_TIME = tendNITMaster.TEND_ISSUE_START_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_ISSUE_START_DATE).ToString("HH:mm"),
                        TEND_ISSUE_END_DATE = tendNITMaster.TEND_ISSUE_END_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_ISSUE_END_DATE).ToString("dd/MM/yyyy"),
                        TEND_ISSUE_END_TIME = tendNITMaster.TEND_ISSUE_END_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_ISSUE_END_DATE).ToString("HH:mm"),
                        TEND_INVITING_AUTHORITY = tendNITMaster.TEND_INVITING_AUTHORITY,
                        TEND_INVITING_ORG = tendNITMaster.TEND_INVITING_ORG,
                        TendItemRate = tendNITMaster.TEND_ITEM_RATE=="I"?true:false,
                        CONT_REGN_WITH_ORG = tendNITMaster.CONT_REGN_WITH_ORG,
                        CONT_REGN_VALIDITY_DATE = tendNITMaster.CONT_REGN_VALIDITY_DATE== null ? string.Empty : Convert.ToDateTime(tendNITMaster.CONT_REGN_VALIDITY_DATE).ToString("dd/MM/yyyy"),
                        CONT_REGN_VALIDITY_TIME = tendNITMaster.CONT_REGN_VALIDITY_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.CONT_REGN_VALIDITY_DATE).ToString("HH:mm"),
                        TEND_DD_ISSUE_IN_FAVOUR = tendNITMaster.TEND_DD_ISSUE_IN_FAVOUR,
                        TEND_DD_PAYABLE_AT = tendNITMaster.TEND_DD_PAYABLE_AT,
                        TEND_AMOUNT_PER_PACKAGE = tendNITMaster.TEND_AMOUNT_PER_PACKAGE,
                        TEND_DOC_INSP_OFFICE = tendNITMaster.TEND_DOC_INSP_OFFICE,
                        TEND_INSP_START_DATE = tendNITMaster.TEND_INSP_START_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_INSP_START_DATE).ToString("dd/MM/yyyy"),
                        TEND_INSP_END_DATE = tendNITMaster.TEND_INSP_END_DATE == null ? string.Empty : Convert.ToDateTime(tendNITMaster.TEND_INSP_END_DATE).ToString("dd/MM/yyyy"),
      
                    };
                }
                return objNITDetails;

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


        public bool UpdateNITDetailsDAL(NITDetails objNITDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
              
                int tendNITCode = 0;
                CommonFunctions commonFunction = new CommonFunctions();


                encryptedParameters = objNITDetails.EncryptedTendNITCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                tendNITCode = Convert.ToInt32(decryptedParameters["TendNITCode"].ToString().Trim());


                if (dbContext.TEND_NIT_MASTER.Any(NIT => NIT.TEND_NIT_NUMBER.ToUpper() == objNITDetails.TEND_NIT_NUMBER.ToUpper() && NIT.TEND_NIT_CODE != tendNITCode))
                {
                    message = "Draft NIT Number is already exist.";
                    return false;
                }


                TEND_NIT_MASTER tendNITMaster = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode).FirstOrDefault();

                if (tendNITMaster == null)
                {
                    return false;
                }
             
                tendNITMaster.TEND_NIT_NUMBER = objNITDetails.TEND_NIT_NUMBER.Trim();

                //if (!string.IsNullOrEmpty(objNITDetails.TEND_ISSUE_START_TIME))
                //{
                //    objNITDetails.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE + " " + objNITDetails.TEND_ISSUE_START_TIME;

                //}
                //if (!string.IsNullOrEmpty(objNITDetails.TEND_ISSUE_END_TIME))
                //{
                //    objNITDetails.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE + " " + objNITDetails.TEND_ISSUE_END_TIME;
                //}
           
                //tendNITMaster.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.TEND_ISSUE_START_DATE);
                //tendNITMaster.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.TEND_ISSUE_END_DATE);

                tendNITMaster.TEND_ISSUE_START_DATE = objNITDetails.TEND_ISSUE_START_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.TEND_ISSUE_START_DATE, objNITDetails.TEND_ISSUE_START_TIME);
                tendNITMaster.TEND_ISSUE_END_DATE = objNITDetails.TEND_ISSUE_END_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.TEND_ISSUE_END_DATE, objNITDetails.TEND_ISSUE_END_TIME);


                tendNITMaster.TEND_INVITING_AUTHORITY = objNITDetails.TEND_INVITING_AUTHORITY == null ? null : objNITDetails.TEND_INVITING_AUTHORITY.Trim();
                tendNITMaster.TEND_INVITING_ORG = objNITDetails.TEND_INVITING_ORG == null ? null : objNITDetails.TEND_INVITING_ORG.Trim();
                tendNITMaster.CONT_REGN_WITH_ORG = objNITDetails.CONT_REGN_WITH_ORG == null ? null : objNITDetails.CONT_REGN_WITH_ORG.Trim();

                //if (!string.IsNullOrEmpty(objNITDetails.CONT_REGN_VALIDITY_TIME))
                //{
                //    objNITDetails.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE + " " + objNITDetails.CONT_REGN_VALIDITY_TIME;
                //}

              
                //tendNITMaster.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITDetails.CONT_REGN_VALIDITY_DATE);

                tendNITMaster.CONT_REGN_VALIDITY_DATE = objNITDetails.CONT_REGN_VALIDITY_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITDetails.CONT_REGN_VALIDITY_DATE, objNITDetails.CONT_REGN_VALIDITY_TIME);

                tendNITMaster.TEND_DD_ISSUE_IN_FAVOUR = objNITDetails.TEND_DD_ISSUE_IN_FAVOUR == null ? null : objNITDetails.TEND_DD_ISSUE_IN_FAVOUR.Trim();
                tendNITMaster.TEND_DD_PAYABLE_AT = objNITDetails.TEND_DD_PAYABLE_AT == null ? null : objNITDetails.TEND_DD_PAYABLE_AT.Trim();

                if (objNITDetails.TEND_AMOUNT_PER_PACKAGE != null)
                {
                    tendNITMaster.TEND_AMOUNT_PER_PACKAGE = (Decimal)objNITDetails.TEND_AMOUNT_PER_PACKAGE;
                }

                tendNITMaster.TEND_DOC_INSP_OFFICE = objNITDetails.TEND_DOC_INSP_OFFICE == null ? null : objNITDetails.TEND_DOC_INSP_OFFICE.Trim();
                tendNITMaster.TEND_INSP_START_DATE = objNITDetails.TEND_INSP_START_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_INSP_START_DATE);
                tendNITMaster.TEND_INSP_END_DATE = objNITDetails.TEND_INSP_END_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_INSP_END_DATE);
                tendNITMaster.TEND_PUBLICATION_DATE = objNITDetails.TEND_PUBLICATION_DATE == null ? null : (DateTime?)commonFunction.GetStringToDateTime(objNITDetails.TEND_PUBLICATION_DATE);
                tendNITMaster.TEND_ITEM_RATE = objNITDetails.TendItemRate == true ? "I" : "P";

                //added by abhishek kamble 27-nov-2013
                tendNITMaster.USERID = PMGSYSession.Current.UserId;
                tendNITMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                tendNITMaster.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Vikram on 10 Feb 2014
                dbContext.Entry(tendNITMaster).State = System.Data.Entity.EntityState.Modified;
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


        public bool DeleteNITDetailsDAL(int tendNITCode, ref string message)
        {
            using (TransactionScope ts =new TransactionScope())
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                try
                {

                    TEND_NIT_MASTER tendNITMaster = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode).FirstOrDefault();

                    if (tendNITMaster == null)
                    {
                        return false;
                    }

                    //added by abhishek kamble 27-nov-2013
                    tendNITMaster.USERID = PMGSYSession.Current.UserId;
                    tendNITMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(tendNITMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.TEND_NIT_MASTER.Remove(tendNITMaster);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;

                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this NIT details.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
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
        }


        public bool PublishNITDAL(int tendNITCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            TEND_NIT_MASTER tendNITMaster = null;
            try
            {
                tendNITMaster = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode && NIT.TEND_PUBLISH_TENDER == "N" && NIT.TEND_LOCK_STATUS == "N").FirstOrDefault();

                if (tendNITMaster == null)
                {
                    return false;
                }

                tendNITMaster.TEND_PUBLISH_TENDER = "Y";

                //added by abhishek kamble 27-nov-2013
                tendNITMaster.USERID = PMGSYSession.Current.UserId;
                tendNITMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                
                dbContext.Entry(tendNITMaster).State = System.Data.Entity.EntityState.Modified;
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public Array GetNITRoadDetailsListDAL(int TendNITCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                var query = from NITMaster in dbContext.TEND_NIT_MASTER
                            join RoadDetails in dbContext.TEND_NIT_DETAILS
                            on NITMaster.TEND_NIT_CODE equals RoadDetails.TEND_NIT_CODE
                            join IMSSanctioned in dbContext.IMS_SANCTIONED_PROJECTS
                            on RoadDetails.IMS_PR_ROAD_CODE equals IMSSanctioned.IMS_PR_ROAD_CODE
                            join IMSWorks in dbContext.IMS_PROPOSAL_WORK 
                            on RoadDetails.IMS_WORK_CODE equals IMSWorks.IMS_WORK_CODE into Works
                            from IMSWorks in Works.DefaultIfEmpty()
                            where
                            RoadDetails.TEND_NIT_CODE==TendNITCode  
                            select new
                            {
                                RoadDetails.TEND_NIT_ID,
                                RoadDetails.TEND_NIT_CODE,
                                RoadDetails.IMS_WORK_CODE,
                                RoadDetails.IMS_PR_ROAD_CODE,
                                IMSSanctioned.IMS_ROAD_NAME,
                                IMSSanctioned.IMS_BRIDGE_NAME,
                                IMSSanctioned.IMS_PROPOSAL_TYPE,
                                IMSWorks.IMS_WORK_DESC,
                                RoadDetails.TEND_RECEVING_DATE,
                                RoadDetails.TEND_OPENING_DATE,
                                RoadDetails.TEND_DATE_OF_TECHNICAL_OPENING,
                                RoadDetails.TEND_DATE_OF_FINANCIAL_OPENING,
                                RoadDetails.TEND_COST_FORM,
                                RoadDetails.TEND_EST_COST,
                                RoadDetails.TEND_MAINT_COST,
                                NITMaster.TEND_PUBLISH_TENDER

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
                            case "WorkName":
                                query = query.OrderBy(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "ReceivingBidsDate":
                                query = query.OrderBy(x => x.TEND_RECEVING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TenderOpeningDate":
                                query = query.OrderBy(x => x.TEND_OPENING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TechnicalBidOpeningDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_TECHNICAL_OPENING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "FinancialBidOpeningDate":
                                query = query.OrderBy(x => x.TEND_DATE_OF_FINANCIAL_OPENING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TenderFormCost":
                                query = query.OrderBy(x => x.TEND_COST_FORM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TotalEstimatedCost":
                                query = query.OrderBy(x => x.TEND_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TotalMaintenanceCost":
                                query = query.OrderBy(x => x.TEND_MAINT_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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
                            case "WorkName":
                                query = query.OrderByDescending(x => x.IMS_WORK_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "ReceivingBidsDate":
                                query = query.OrderByDescending(x => x.TEND_RECEVING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TenderOpeningDate":
                                query = query.OrderByDescending(x => x.TEND_OPENING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TechnicalBidOpeningDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_TECHNICAL_OPENING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "FinancialBidOpeningDate":
                                query = query.OrderByDescending(x => x.TEND_DATE_OF_FINANCIAL_OPENING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TenderFormCost":
                                query = query.OrderByDescending(x => x.TEND_COST_FORM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TotalEstimatedCost":
                                query = query.OrderByDescending(x => x.TEND_EST_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "TotalMaintenanceCost":
                                query = query.OrderByDescending(x => x.TEND_MAINT_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
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

                var result = query.Select(RoadDetails => new
                {

                    RoadDetails.TEND_NIT_ID,
                    RoadDetails.TEND_NIT_CODE,
                    RoadDetails.IMS_WORK_CODE,
                    RoadDetails.IMS_PR_ROAD_CODE,
                    RoadDetails.IMS_ROAD_NAME,
                    RoadDetails.IMS_BRIDGE_NAME,
                    RoadDetails.IMS_PROPOSAL_TYPE,
                    RoadDetails.IMS_WORK_DESC,
                    RoadDetails.TEND_RECEVING_DATE,
                    RoadDetails.TEND_OPENING_DATE,
                    RoadDetails.TEND_DATE_OF_TECHNICAL_OPENING,
                    RoadDetails.TEND_DATE_OF_FINANCIAL_OPENING,
                    RoadDetails.TEND_COST_FORM,
                    RoadDetails.TEND_EST_COST,
                    RoadDetails.TEND_MAINT_COST,
                    RoadDetails.TEND_PUBLISH_TENDER


                }).ToArray();


                return result.Select(RoadDetails => new
                {
                    //id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {  
                                    RoadDetails.IMS_ROAD_NAME.ToString(),   
                                    RoadDetails.IMS_WORK_CODE==null?"NA":RoadDetails.IMS_WORK_DESC.ToString(),                                              
                                    RoadDetails.TEND_RECEVING_DATE==null?"NA":Convert.ToDateTime(RoadDetails.TEND_RECEVING_DATE).ToString("dd/MM/yyyy HH:mm"),
                                    RoadDetails.TEND_OPENING_DATE==null?"NA":Convert.ToDateTime(RoadDetails.TEND_OPENING_DATE).ToString("dd/MM/yyyy HH:mm"),
                                    RoadDetails.TEND_DATE_OF_TECHNICAL_OPENING==null?"NA":Convert.ToDateTime(RoadDetails.TEND_DATE_OF_TECHNICAL_OPENING).ToString("dd/MM/yyyy HH:mm"),
                                    RoadDetails.TEND_DATE_OF_FINANCIAL_OPENING==null?"NA":Convert.ToDateTime(RoadDetails.TEND_DATE_OF_FINANCIAL_OPENING).ToString("dd/MM/yyyy HH:mm"),
                                    RoadDetails.TEND_COST_FORM.ToString(),
                                    RoadDetails.TEND_EST_COST==null?"NA": RoadDetails.TEND_EST_COST.ToString(),
                                    RoadDetails.TEND_MAINT_COST==null?"NA": RoadDetails.TEND_MAINT_COST.ToString(),                                                                                               
                                    URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + RoadDetails.TEND_NIT_CODE.ToString(),"TendNITID =" + RoadDetails.TEND_NIT_ID.ToString()  }),
                                    RoadDetails.TEND_PUBLISH_TENDER.Trim()=="Y"?string.Empty: URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + RoadDetails.TEND_NIT_CODE.ToString(),"TendNITID =" + RoadDetails.TEND_NIT_ID.ToString()  }),
                                    RoadDetails.TEND_PUBLISH_TENDER.Trim()=="Y"?string.Empty: URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + RoadDetails.TEND_NIT_CODE.ToString(),"TendNITID =" + RoadDetails.TEND_NIT_ID.ToString()  })
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


        public bool SaveNITRoadDetailsDAL(NITRoadDetails objNITRoadDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int TendNITCode = 0;
                TEND_NIT_DETAILS tendNITDetails = null;
                encryptedParameters = objNITRoadDetails.EncryptedTendNITCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                TendNITCode = Convert.ToInt32(decryptedParameters["TendNITCode"].ToString().Trim());

                if (objNITRoadDetails.IMS_WORK_CODE > 0)
                {
                    if (dbContext.TEND_NIT_DETAILS.Any(NIT => NIT.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE && NIT.IMS_WORK_CODE == objNITRoadDetails.IMS_WORK_CODE))
                    {
                        message = "Road details with selected work is already exist.";
                        return false;
                    }
                    
                }
                else 
                {
                    if (dbContext.TEND_NIT_DETAILS.Any(NIT => NIT.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE))
                    {
                        message = "Road details is already exist.";
                        return false;
                    }
                }

                if (!CheckCost(objNITRoadDetails, ref message ))
                {
                   // message = "Total Estimated Cost and Total Maintenance Cost should be same as sanctioned total cost and maintenance cost for selected work.";
                    return false;
                }

                tendNITDetails = new TEND_NIT_DETAILS();

                tendNITDetails.TEND_NIT_ID = (Int32)GetMaxCode(NITModules.NITDetails);
                tendNITDetails.TEND_NIT_CODE = TendNITCode;
                tendNITDetails.IMS_PR_ROAD_CODE = objNITRoadDetails.IMS_PR_ROAD_CODE;
                if (objNITRoadDetails.IMS_WORK_CODE > 0)
                {
                    tendNITDetails.IMS_WORK_CODE = objNITRoadDetails.IMS_WORK_CODE;
                }

                tendNITDetails.TEND_COST_FORM = (Decimal)objNITRoadDetails.TEND_COST_FORM;
                tendNITDetails.MAST_CON_CLASS = objNITRoadDetails.MAST_CON_CLASS;

                tendNITDetails.TEND_EARNEST_MONEY = (Decimal)objNITRoadDetails.TEND_EARNEST_MONEY;



                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_RECEVING_TIME))
                //{
                //    objNITRoadDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE + " " + objNITRoadDetails.TEND_RECEVING_TIME;

                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_OPENING_TIME))
                //{
                //    objNITRoadDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE + " " + objNITRoadDetails.TEND_OPENING_TIME;
                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING + " " + objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING;
                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING + " " + objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING;
                //}

                //tendNITDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_RECEVING_DATE);
                //tendNITDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_OPENING_DATE);
                //tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING);
                //tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING);

                tendNITDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_RECEVING_DATE,objNITRoadDetails.TEND_RECEVING_TIME);
                tendNITDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_OPENING_DATE,objNITRoadDetails.TEND_OPENING_TIME);
                tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING,objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING);
                tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING,objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING);


                tendNITDetails.TEND_PLACE_OF_SALE = objNITRoadDetails.TEND_PLACE_OF_SALE == null ? null : objNITRoadDetails.TEND_PLACE_OF_SALE.Trim();
                tendNITDetails.TEND_RECEIVE_AUTH = objNITRoadDetails.TEND_RECEIVE_AUTH == null ? null : objNITRoadDetails.TEND_RECEIVE_AUTH.Trim();

                tendNITDetails.TEND_PRE_BID_DETAILS = objNITRoadDetails.TEND_PRE_BID_DETAILS == null ? null : objNITRoadDetails.TEND_PRE_BID_DETAILS.Trim();


                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_PREBID))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID + " " + objNITRoadDetails.TEND_TIME_OF_PREBID;
                //}

                //tendNITDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_PREBID);

                tendNITDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_PREBID, objNITRoadDetails.TEND_TIME_OF_PREBID);

                tendNITDetails.TEND_PLACE_OF_OPENING = objNITRoadDetails.TEND_PLACE_OF_OPENING == null ? null : objNITRoadDetails.TEND_PLACE_OF_OPENING.Trim();

                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY + " " + objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY;
                //}

                //tendNITDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY);

                tendNITDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY, objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY);

                tendNITDetails.TEND_EST_COST = objNITRoadDetails.TEND_EST_COST;
                tendNITDetails.TEND_COMPLETION_TIME = objNITRoadDetails.TEND_COMPLETION_TIME;
                tendNITDetails.TEND_MAINT_COST = objNITRoadDetails.TEND_MAINT_COST;
                tendNITDetails.TEND_SITE_CONTACT_PERSON_NAME = objNITRoadDetails.TEND_SITE_CONTACT_PERSON_NAME == null ? null : objNITRoadDetails.TEND_SITE_CONTACT_PERSON_NAME.Trim();
                tendNITDetails.TEND_SITE_CONTACT_PERSON_ADDRESS = objNITRoadDetails.TEND_SITE_CONTACT_PERSON_ADDRESS == null ? null : objNITRoadDetails.TEND_SITE_CONTACT_PERSON_ADDRESS.Trim();
                tendNITDetails.TEND_TELE_CODE = objNITRoadDetails.TEND_TELE_CODE == null ? null : objNITRoadDetails.TEND_TELE_CODE.Trim();
                tendNITDetails.TEND_TELE_NUMBER = objNITRoadDetails.TEND_TELE_NUMBER == null ? null : objNITRoadDetails.TEND_TELE_NUMBER.Trim();
                tendNITDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF = objNITRoadDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF == null ? null : objNITRoadDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF.Trim();
                tendNITDetails.TEND_EXTRA_COST_FOR_POST = objNITRoadDetails.TEND_EXTRA_COST_FOR_POST == null ? null : objNITRoadDetails.TEND_EXTRA_COST_FOR_POST.Trim();
                tendNITDetails.TEND_ENGINEER_FOR_CONTRACT = objNITRoadDetails.TEND_ENGINEER_FOR_CONTRACT == null ? null : objNITRoadDetails.TEND_ENGINEER_FOR_CONTRACT.Trim();
                tendNITDetails.TEND_OTHER_SUBMISSION_PLACES = objNITRoadDetails.TEND_OTHER_SUBMISSION_PLACES == null ? null : objNITRoadDetails.TEND_OTHER_SUBMISSION_PLACES.Trim();
                tendNITDetails.TEND_SECTION_COMPLETION = objNITRoadDetails.TEND_SECTION_COMPLETION == null ? null : objNITRoadDetails.TEND_SECTION_COMPLETION.Trim();
                tendNITDetails.TEND_SITE_INVESTIGATION_REPORT = objNITRoadDetails.TEND_SITE_INVESTIGATION_REPORT == null ? null : objNITRoadDetails.TEND_SITE_INVESTIGATION_REPORT.Trim();


                //added by abhishek kamble 27-nov-2013
                tendNITDetails.USERID = PMGSYSession.Current.UserId;
                tendNITDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                
                dbContext.TEND_NIT_DETAILS.Add(tendNITDetails);
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

        private bool CheckCost(NITRoadDetails objNITRoadDetails, ref string message)
        {  
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                decimal estimatedCost = 0;
                decimal maintenanceCost = 0;


                if (objNITRoadDetails.IMS_WORK_CODE == 0)
                {
                    IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE).FirstOrDefault();

                    if (IMSSanctioned != null)
                    {

                        if (IMSSanctioned.IMS_PROPOSAL_TYPE.Equals("P"))
                        {
                            estimatedCost = (IMSSanctioned.IMS_SANCTIONED_PAV_AMT + IMSSanctioned.IMS_SANCTIONED_CD_AMT + IMSSanctioned.IMS_SANCTIONED_PW_AMT + IMSSanctioned.IMS_SANCTIONED_OW_AMT + IMSSanctioned.IMS_SANCTIONED_RS_AMT);

                            maintenanceCost = (IMSSanctioned.IMS_SANCTIONED_MAN_AMT1 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT2 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT3 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT4 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT5);

                            if (estimatedCost != objNITRoadDetails.TEND_EST_COST && maintenanceCost!=objNITRoadDetails.TEND_MAINT_COST)
                            {
                                message = "Total Estimated Cost and Total Maintenance Cost should be same as sanctioned total cost and maintenance cost for selected road.";
                                return false;
                            }
                            else if (estimatedCost != objNITRoadDetails.TEND_EST_COST )
                            {
                                message = "Total Estimated Cost should be same as sanctioned total cost for selected road.";
                                return false;
                            }
                            else if (maintenanceCost != objNITRoadDetails.TEND_MAINT_COST)
                            {
                                message = "Total Maintenance Cost should be same as sanctioned total maintenance cost for selected road.";
                                return false;
                            }

                        }
                        else
                        {
                            estimatedCost = (IMSSanctioned.IMS_SANCTIONED_BW_AMT + IMSSanctioned.IMS_SANCTIONED_BS_AMT);

                            if (estimatedCost != objNITRoadDetails.TEND_EST_COST)
                            {
                                message = "Total Estimated Cost should be same as sanctioned total cost for selected road.";
                                return false;
                            }
                            else if (objNITRoadDetails.TEND_MAINT_COST != 0)
                            {
                                message = "Total Maintenance Cost should be same as sanctioned total maintenance cost for selected road.";
                                return false;
                            }
                        }

                      
                    }
                }
                else if (objNITRoadDetails.IMS_WORK_CODE > 0)
                {
                    IMS_PROPOSAL_WORK IMSProposalWork = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE && pw.IMS_WORK_CODE == objNITRoadDetails.IMS_WORK_CODE).FirstOrDefault();

                    if (IMSProposalWork != null)
                    {

                        estimatedCost = (IMSProposalWork.IMS_PAV_EST_COST + IMSProposalWork.IMS_CD_WORKS_EST_COST + IMSProposalWork.IMS_PROTECTION_WORKS + IMSProposalWork.IMS_OTHER_WORK_COST + IMSProposalWork.IMS_STATE_SHARE);

                        maintenanceCost = (IMSProposalWork.IMS_MAINTENANCE_YEAR1 + IMSProposalWork.IMS_MAINTENANCE_YEAR2 + IMSProposalWork.IMS_MAINTENANCE_YEAR3 + IMSProposalWork.IMS_MAINTENANCE_YEAR4 + IMSProposalWork.IMS_MAINTENANCE_YEAR5);

                        if (estimatedCost != objNITRoadDetails.TEND_EST_COST && maintenanceCost != objNITRoadDetails.TEND_MAINT_COST)
                        {
                            message = "Total Estimated Cost and Total Maintenance Cost should be same as sanctioned total cost and maintenance cost for selected road.";
                            return false;
                        }
                        else if (estimatedCost != objNITRoadDetails.TEND_EST_COST)
                        {
                            message = "Total Estimated Cost should be same as sanctioned total cost for selected road.";
                            return false;
                        }
                        else if (maintenanceCost != objNITRoadDetails.TEND_MAINT_COST)
                        {
                            message = "Total Maintenance Cost should be same as sanctioned total maintenance cost for selected road.";
                            return false;
                        }

                    }
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


        public NITRoadDetails GetNITRoadDetailsDAL(int tendNITID, int tendNITCode, bool isView = false)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions commonFunction = new CommonFunctions();

                TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode && NIT.TEND_NIT_ID == tendNITID).FirstOrDefault();
                
                IMS_SANCTIONED_PROJECTS imsSanctionedProjects = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == tendNITDetails.IMS_PR_ROAD_CODE).FirstOrDefault();

                NITRoadDetails objNITRoadDetails = null;

                DateTime? formIssueDate = dbContext.TEND_NIT_MASTER.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode).Select(NIT => NIT.TEND_ISSUE_START_DATE).FirstOrDefault();
            

                if (tendNITDetails != null && imsSanctionedProjects!=null)
                {

                    objNITRoadDetails = new NITRoadDetails()
                    {

                        EncryptedTendNITCode = URLEncrypt.EncryptParameters1(new string[] { "TendNITCode =" + tendNITDetails.TEND_NIT_CODE.ToString(), "TendNITID =" + tendNITDetails.TEND_NIT_ID.ToString() }),
                        EncryptedTendNITID = URLEncrypt.EncryptParameters1(new string[] { "TendNITID =" + tendNITDetails.TEND_NIT_ID.ToString() }),
                        SanctionYear=imsSanctionedProjects.IMS_YEAR,
                        PackageID = imsSanctionedProjects.IMS_PACKAGE_ID,
                        IMS_PR_ROAD_CODE=tendNITDetails.IMS_PR_ROAD_CODE,
                        IMS_WORK_CODE = tendNITDetails.IMS_WORK_CODE == null ? 0 : tendNITDetails.IMS_WORK_CODE,
                        TEND_COST_FORM = tendNITDetails.TEND_COST_FORM,
                        MAST_CON_CLASS=tendNITDetails.MAST_CON_CLASS,
                        TEND_EARNEST_MONEY = tendNITDetails.TEND_EARNEST_MONEY,

                        TEND_RECEVING_DATE = tendNITDetails.TEND_RECEVING_DATE == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_RECEVING_DATE).ToString("dd/MM/yyyy"),
                        TEND_RECEVING_TIME = tendNITDetails.TEND_RECEVING_DATE == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_RECEVING_DATE).ToString("HH:mm"),

                        TEND_OPENING_DATE = tendNITDetails.TEND_OPENING_DATE == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_OPENING_DATE).ToString("dd/MM/yyyy"),
                        TEND_OPENING_TIME = tendNITDetails.TEND_OPENING_DATE == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_OPENING_DATE).ToString("HH:mm"),


                        TEND_DATE_OF_TECHNICAL_OPENING = tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING).ToString("dd/MM/yyyy"),
                        TEND_TIME_OF_TECHNICAL_OPENING = tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING).ToString("HH:mm"),


                        TEND_DATE_OF_FINANCIAL_OPENING = tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING).ToString("dd/MM/yyyy"),
                        TEND_TIME_OF_FINANCIAL_OPENING = tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING).ToString("HH:mm"),


                        TEND_PLACE_OF_SALE = tendNITDetails.TEND_PLACE_OF_SALE,
                        TEND_RECEIVE_AUTH = tendNITDetails.TEND_RECEIVE_AUTH,
                        TEND_PRE_BID_DETAILS = tendNITDetails.TEND_PRE_BID_DETAILS,

                        TEND_DATE_OF_PREBID = tendNITDetails.TEND_DATE_OF_PREBID == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_PREBID).ToString("dd/MM/yyyy"),
                        TEND_TIME_OF_PREBID = tendNITDetails.TEND_DATE_OF_PREBID == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_PREBID).ToString("HH:mm"),

                        TEND_PLACE_OF_OPENING = tendNITDetails.TEND_PLACE_OF_OPENING,

                        TEND_DATE_OF_BID_VALIDITY = tendNITDetails.TEND_DATE_OF_BID_VALIDITY == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_BID_VALIDITY).ToString("dd/MM/yyyy"),
                        TEND_TIME_OF_BID_VALIDITY = tendNITDetails.TEND_DATE_OF_BID_VALIDITY == null ? string.Empty : Convert.ToDateTime(tendNITDetails.TEND_DATE_OF_BID_VALIDITY).ToString("HH:mm"),

                        TEND_EST_COST = tendNITDetails.TEND_EST_COST,
                        TEND_COMPLETION_TIME = tendNITDetails.TEND_COMPLETION_TIME,
                        TEND_MAINT_COST = tendNITDetails.TEND_MAINT_COST,

                        TEND_SITE_CONTACT_PERSON_NAME = tendNITDetails.TEND_SITE_CONTACT_PERSON_NAME,
                        TEND_SITE_CONTACT_PERSON_ADDRESS = tendNITDetails.TEND_SITE_CONTACT_PERSON_ADDRESS,
                        TEND_TELE_CODE = tendNITDetails.TEND_TELE_CODE,
                        TEND_TELE_NUMBER = tendNITDetails.TEND_TELE_NUMBER,
                        TEND_PLEDGED_EARN_MONEY_NAMEOF = tendNITDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF,
                        TEND_EXTRA_COST_FOR_POST = tendNITDetails.TEND_EXTRA_COST_FOR_POST,
                        TEND_ENGINEER_FOR_CONTRACT = tendNITDetails.TEND_ENGINEER_FOR_CONTRACT,
                        TEND_OTHER_SUBMISSION_PLACES = tendNITDetails.TEND_OTHER_SUBMISSION_PLACES,
                        TEND_SECTION_COMPLETION = tendNITDetails.TEND_SECTION_COMPLETION,
                        TEND_SITE_INVESTIGATION_REPORT = tendNITDetails.TEND_SITE_INVESTIGATION_REPORT,
                        TenderIssueStartDate = formIssueDate == null ? null : Convert.ToDateTime(formIssueDate).ToString("dd/MM/yyyy")

                        

                    };
                }
                return objNITRoadDetails;

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


        public bool UpdateNITRoadDetailsDAL(NITRoadDetails objNITRoadDetails, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int tendNITCode = 0;
                int tendNITID = 0;
                CommonFunctions commonFunction = new CommonFunctions();


                encryptedParameters = objNITRoadDetails.EncryptedTendNITCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                tendNITCode = Convert.ToInt32(decryptedParameters["TendNITCode"].ToString().Trim());
                tendNITID = Convert.ToInt32(decryptedParameters["TendNITID"].ToString().Trim());


                if (objNITRoadDetails.IMS_WORK_CODE > 0)
                {
                    if (dbContext.TEND_NIT_DETAILS.Any(NIT => NIT.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE && NIT.IMS_WORK_CODE == objNITRoadDetails.IMS_WORK_CODE && NIT.TEND_NIT_ID!=tendNITID))
                    {
                        message = "Road details with selected work is already exist.";
                        return false;
                    }

                }
                else
                {
                    if (dbContext.TEND_NIT_DETAILS.Any(NIT => NIT.IMS_PR_ROAD_CODE == objNITRoadDetails.IMS_PR_ROAD_CODE &&   NIT.TEND_NIT_ID!=tendNITID))
                    {
                        message = "Road details is already exist.";
                        return false;
                    }
                }

                if (!CheckCost(objNITRoadDetails, ref message))
                {
                    // message = "Total Estimated Cost and Total Maintenance Cost should be same as sanctioned total cost and maintenance cost for selected work.";
                    return false;
                }

                TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode && NIT.TEND_NIT_ID == tendNITID).FirstOrDefault();

                if (tendNITDetails == null)
                {
                    return false;
                }

                tendNITDetails.TEND_COST_FORM = (Decimal)objNITRoadDetails.TEND_COST_FORM;
                tendNITDetails.MAST_CON_CLASS = objNITRoadDetails.MAST_CON_CLASS;

                tendNITDetails.TEND_EARNEST_MONEY = (Decimal)objNITRoadDetails.TEND_EARNEST_MONEY;



                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_RECEVING_TIME))
                //{
                //    objNITRoadDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE + " " + objNITRoadDetails.TEND_RECEVING_TIME;

                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_OPENING_TIME))
                //{
                //    objNITRoadDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE + " " + objNITRoadDetails.TEND_OPENING_TIME;
                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING + " " + objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING;
                //}
                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING + " " + objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING;
                //}

                //tendNITDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_RECEVING_DATE);
                //tendNITDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_OPENING_DATE);
                //tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING);
                //tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING);


                tendNITDetails.TEND_RECEVING_DATE = objNITRoadDetails.TEND_RECEVING_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_RECEVING_DATE, objNITRoadDetails.TEND_RECEVING_TIME);
                tendNITDetails.TEND_OPENING_DATE = objNITRoadDetails.TEND_OPENING_DATE == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_OPENING_DATE, objNITRoadDetails.TEND_OPENING_TIME);
                tendNITDetails.TEND_DATE_OF_TECHNICAL_OPENING = objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_TECHNICAL_OPENING, objNITRoadDetails.TEND_TIME_OF_TECHNICAL_OPENING);
                tendNITDetails.TEND_DATE_OF_FINANCIAL_OPENING = objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_FINANCIAL_OPENING, objNITRoadDetails.TEND_TIME_OF_FINANCIAL_OPENING);


                tendNITDetails.TEND_PLACE_OF_SALE = objNITRoadDetails.TEND_PLACE_OF_SALE == null ? null : objNITRoadDetails.TEND_PLACE_OF_SALE.Trim();
                tendNITDetails.TEND_RECEIVE_AUTH = objNITRoadDetails.TEND_RECEIVE_AUTH == null ? null : objNITRoadDetails.TEND_RECEIVE_AUTH.Trim();

                tendNITDetails.TEND_PRE_BID_DETAILS = objNITRoadDetails.TEND_PRE_BID_DETAILS == null ? null : objNITRoadDetails.TEND_PRE_BID_DETAILS.Trim();


                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_PREBID))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID + " " + objNITRoadDetails.TEND_TIME_OF_PREBID;
                //}

                //tendNITDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_PREBID);

                tendNITDetails.TEND_DATE_OF_PREBID = objNITRoadDetails.TEND_DATE_OF_PREBID == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_PREBID, objNITRoadDetails.TEND_TIME_OF_PREBID);

                tendNITDetails.TEND_PLACE_OF_OPENING = objNITRoadDetails.TEND_PLACE_OF_OPENING == null ? null : objNITRoadDetails.TEND_PLACE_OF_OPENING.Trim();

                //if (!string.IsNullOrEmpty(objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY))
                //{
                //    objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY + " " + objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY;
                //}

                //tendNITDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY == null ? null : (DateTime?)Convert.ToDateTime(objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY);

                tendNITDetails.TEND_DATE_OF_BID_VALIDITY = objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY == null ? null : (DateTime?)GetStringToDateTime(objNITRoadDetails.TEND_DATE_OF_BID_VALIDITY, objNITRoadDetails.TEND_TIME_OF_BID_VALIDITY);

                tendNITDetails.TEND_EST_COST = objNITRoadDetails.TEND_EST_COST;
                tendNITDetails.TEND_COMPLETION_TIME = objNITRoadDetails.TEND_COMPLETION_TIME;
                tendNITDetails.TEND_MAINT_COST = objNITRoadDetails.TEND_MAINT_COST;
                tendNITDetails.TEND_SITE_CONTACT_PERSON_NAME = objNITRoadDetails.TEND_SITE_CONTACT_PERSON_NAME == null ? null : objNITRoadDetails.TEND_SITE_CONTACT_PERSON_NAME.Trim();
                tendNITDetails.TEND_SITE_CONTACT_PERSON_ADDRESS = objNITRoadDetails.TEND_SITE_CONTACT_PERSON_ADDRESS == null ? null : objNITRoadDetails.TEND_SITE_CONTACT_PERSON_ADDRESS.Trim();
                tendNITDetails.TEND_TELE_CODE = objNITRoadDetails.TEND_TELE_CODE == null ? null : objNITRoadDetails.TEND_TELE_CODE.Trim();
                tendNITDetails.TEND_TELE_NUMBER = objNITRoadDetails.TEND_TELE_NUMBER == null ? null : objNITRoadDetails.TEND_TELE_NUMBER.Trim();
                tendNITDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF = objNITRoadDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF == null ? null : objNITRoadDetails.TEND_PLEDGED_EARN_MONEY_NAMEOF.Trim();
                tendNITDetails.TEND_EXTRA_COST_FOR_POST = objNITRoadDetails.TEND_EXTRA_COST_FOR_POST == null ? null : objNITRoadDetails.TEND_EXTRA_COST_FOR_POST.Trim();
                tendNITDetails.TEND_ENGINEER_FOR_CONTRACT = objNITRoadDetails.TEND_ENGINEER_FOR_CONTRACT == null ? null : objNITRoadDetails.TEND_ENGINEER_FOR_CONTRACT.Trim();
                tendNITDetails.TEND_OTHER_SUBMISSION_PLACES = objNITRoadDetails.TEND_OTHER_SUBMISSION_PLACES == null ? null : objNITRoadDetails.TEND_OTHER_SUBMISSION_PLACES.Trim();
                tendNITDetails.TEND_SECTION_COMPLETION = objNITRoadDetails.TEND_SECTION_COMPLETION == null ? null : objNITRoadDetails.TEND_SECTION_COMPLETION.Trim();
                tendNITDetails.TEND_SITE_INVESTIGATION_REPORT = objNITRoadDetails.TEND_SITE_INVESTIGATION_REPORT == null ? null : objNITRoadDetails.TEND_SITE_INVESTIGATION_REPORT.Trim();



                //added by abhishek kamble 27-nov-2013
                tendNITDetails.USERID = PMGSYSession.Current.UserId;
                tendNITDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                

                dbContext.Entry(tendNITDetails).State = System.Data.Entity.EntityState.Modified;
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


        public bool DeleteNITRoadDetailsDAL(int tendNITID, int tendNITCode, ref string message)
        {

            using(TransactionScope ts=new TransactionScope()){
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.TEND_NIT_CODE == tendNITCode && NIT.TEND_NIT_ID==tendNITID).FirstOrDefault();

                if (tendNITDetails == null)
                {
                    return false;
                }


                //added by abhishek kamble 27-nov-2013
                tendNITDetails.USERID = PMGSYSession.Current.UserId;
                tendNITDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(tendNITDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.TEND_NIT_DETAILS.Remove(tendNITDetails);
                dbContext.SaveChanges();
                ts.Complete();
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ts.Dispose();
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this road details.";
                return false;
            }
            catch (Exception ex)
            {
                ts.Dispose();
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }   }
        }

        private DateTime GetStringToDateTime(string strDate,string strTime)
        {
             
            string[] formats = { "dd/MM/yyyy" };
            DateTime dateTime = DateTime.ParseExact(strDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            if (!string.IsNullOrEmpty(strTime))
            {
                dateTime = dateTime.Add(TimeSpan.Parse(strTime));
            }

            return dateTime;
        }


        public void GetEstimatedCostMaintenanceCostDAL(int roadCode,int workCode, ref string totalEstimatedCost, ref string totalMaintenanceCost)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            decimal estimatedCost = 0;
            decimal maintenanceCost = 0;
            try
            {
                if (workCode == 0)
                {
                    IMS_SANCTIONED_PROJECTS IMSSanctioned = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();

                    if (IMSSanctioned != null)
                    {
                        if (IMSSanctioned.IMS_PROPOSAL_TYPE.Equals("P"))
                        {
                            if (PMGSYSession.Current.PMGSYScheme == 1)
                            {
                                estimatedCost = (IMSSanctioned.IMS_SANCTIONED_PAV_AMT + IMSSanctioned.IMS_SANCTIONED_CD_AMT + IMSSanctioned.IMS_SANCTIONED_PW_AMT + IMSSanctioned.IMS_SANCTIONED_OW_AMT + IMSSanctioned.IMS_SANCTIONED_RS_AMT);
                                maintenanceCost = (IMSSanctioned.IMS_SANCTIONED_MAN_AMT1 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT2 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT3 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT4 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT5);
                            }
                            //change done by Vikram on 05 April 2014 - if scheme 2 then add higher specification cost and furniture cost in estimated cost
                            else if (PMGSYSession.Current.PMGSYScheme == 2)
                            {
                                estimatedCost = (IMSSanctioned.IMS_SANCTIONED_PAV_AMT + IMSSanctioned.IMS_SANCTIONED_CD_AMT + IMSSanctioned.IMS_SANCTIONED_PW_AMT + IMSSanctioned.IMS_SANCTIONED_OW_AMT + IMSSanctioned.IMS_HIGHER_SPECIFICATION_COST.Value + IMSSanctioned.IMS_FURNITURE_COST.Value);
                                maintenanceCost = (IMSSanctioned.IMS_SANCTIONED_MAN_AMT1 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT2 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT3 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT4 + IMSSanctioned.IMS_SANCTIONED_MAN_AMT5 + IMSSanctioned.IMS_SANCTIONED_RENEWAL_AMT.Value);
                            }
                        }
                        else
                        {
                            if (PMGSYSession.Current.PMGSYScheme == 1)
                            {
                                estimatedCost = (IMSSanctioned.IMS_SANCTIONED_BW_AMT + IMSSanctioned.IMS_SANCTIONED_BS_AMT);
                            }
                            else if (PMGSYSession.Current.PMGSYScheme == 2)
                            {
                                estimatedCost = (IMSSanctioned.IMS_SANCTIONED_BW_AMT);
                            }
                        }

                        totalEstimatedCost = estimatedCost.ToString("F");
                        totalMaintenanceCost = maintenanceCost.ToString("F");
                    }
                }
                else if (workCode > 0)
                {
                    IMS_PROPOSAL_WORK IMSProposalWork = dbContext.IMS_PROPOSAL_WORK.Where(pw => pw.IMS_PR_ROAD_CODE == roadCode && pw.IMS_WORK_CODE == workCode).FirstOrDefault();

                    if (IMSProposalWork != null)
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            estimatedCost = (IMSProposalWork.IMS_PAV_EST_COST + IMSProposalWork.IMS_CD_WORKS_EST_COST + IMSProposalWork.IMS_PROTECTION_WORKS + IMSProposalWork.IMS_OTHER_WORK_COST + IMSProposalWork.IMS_STATE_SHARE);
                            maintenanceCost = (IMSProposalWork.IMS_MAINTENANCE_YEAR1 + IMSProposalWork.IMS_MAINTENANCE_YEAR2 + IMSProposalWork.IMS_MAINTENANCE_YEAR3 + IMSProposalWork.IMS_MAINTENANCE_YEAR4 + IMSProposalWork.IMS_MAINTENANCE_YEAR5);
                        }
                        else if (PMGSYSession.Current.PMGSYScheme == 2)
                        {
                            estimatedCost = (IMSProposalWork.IMS_PAV_EST_COST + IMSProposalWork.IMS_CD_WORKS_EST_COST + IMSProposalWork.IMS_PROTECTION_WORKS + IMSProposalWork.IMS_OTHER_WORK_COST + IMSProposalWork.IMS_HIGHER_SPECIFICATION_COST.Value + IMSProposalWork.IMS_FURNITURE_COST.Value);
                            maintenanceCost = (IMSProposalWork.IMS_MAINTENANCE_YEAR1 + IMSProposalWork.IMS_MAINTENANCE_YEAR2 + IMSProposalWork.IMS_MAINTENANCE_YEAR3 + IMSProposalWork.IMS_MAINTENANCE_YEAR4 + IMSProposalWork.IMS_MAINTENANCE_YEAR5 + IMSProposalWork.IMS_MAINTENANCE_YEAR6.Value);
                        }

                        totalEstimatedCost = estimatedCost.ToString("F");
                        totalMaintenanceCost = maintenanceCost.ToString("F");
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }
    }

    public interface INITDAL
    {

        Array GetNITDetailsDAL(int stateCode, int districtCode, int adminNDCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveNITDetailsDAL(NITDetails objNITDetails, ref string message);

        NITDetails GetNITDetailsDAL(int tendNITCode);

        bool UpdateNITDetailsDAL(NITDetails objNITDetails, ref string message);

        bool DeleteNITDetailsDAL(int tendNITCode, ref string message);

        bool PublishNITDAL(int tendNITCode);

        Array GetNITRoadDetailsListDAL(int TendNITCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveNITRoadDetailsDAL(NITRoadDetails objNITRoadDetails, ref string message);

        NITRoadDetails GetNITRoadDetailsDAL(int tendNITID, int tendNITCode, bool isView = false);

        bool UpdateNITRoadDetailsDAL(NITRoadDetails objNITRoadDetails, ref string message);

        bool DeleteNITRoadDetailsDAL(int tendNITID, int tendNITCode, ref string message);

        void GetEstimatedCostMaintenanceCostDAL(int roadCode,int workCode, ref string totalEstimatedCost, ref string totalMaintenanceCost);
    }
}