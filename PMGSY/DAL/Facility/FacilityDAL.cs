using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.DAL.Facility
{
    public class FacilityDAL
    {
        PMGSYEntities dbContext;
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;

        #region Finalize Facility PMGSY3 BLOCK/DISTRICT

        public Array GetBlockListPMGSY3DAL(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized, ref bool isDistrictDefinalized)
        {
            try
            {
                isAllBlockFinalized = false;
                isDistrictDefinalized = false;

                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<PMGSY.Common.CommonFunctions.SearchJson>(filters);

                    foreach (PMGSY.Common.CommonFunctions.rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MAST_ER_ROAD_NAME": roadName = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();

                var lstFacilityFinalizedBlocks = dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_DISTRICT_CODE == districtCode && c.MAST_BLOCK_ACTIVE == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim() }).OrderBy(z => z.MAST_BLOCK_CODE).Distinct().ToList();

                totalRecords = lstBlock.Count();

                if (lstBlock.Count() == lstFacilityFinalizedBlocks.Count() && (!dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") || !dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
                {
                    isAllBlockFinalized = true;
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstBlock = lstBlock.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE DistrictFinalizedDetails = dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();
                if (PMGSYSession.Current.RoleCode == 25 && DistrictFinalizedDetails == null)
                {
                    isDistrictDefinalized = true;
                    return lstBlock.Select(item => new
                    {
                        //id = item.MAST_ER_ROAD_CODE,
                        cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y").Any())
                        ? "<a href='#' title='Click here to definalize details' class='ui-icon ui-icon-locked ui-align-center' onClick =DefinalizeFacilityBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                        //: (dbContext.FACILITY_HABITATION_MAPPING.Any()) ? "All Facility roads not finalized for the block"
                        : (from hm in dbContext.FACILITY_HABITATION_MAPPING join mf in dbContext.MASTER_FACILITY on hm.MASTER_FACILITY_ID equals mf.MASTER_FACILITY_ID select new { hm.MASTER_BLOCK_CODE, mf.IS_FINALIZED }).Where(c=>c.MASTER_BLOCK_CODE == item.MAST_BLOCK_CODE && (c.IS_FINALIZED == "N" || c.IS_FINALIZED == null)).Any()
                        ? "All Facilities are not finalized for the block"
                                : (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "N").Any() || !(dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any()))
                                    ? "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeFacilityBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                            
                                    : ""
                    }
                    }).ToArray();
                }
                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y").Any())
                        ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Locked'></span>"
                        //: (dbContext.FACILITY_HABITATION_MAPPING.Any()) ? "All Facility roads not finalized for the block"
                        : (from hm in dbContext.FACILITY_HABITATION_MAPPING join mf in dbContext.MASTER_FACILITY on hm.MASTER_FACILITY_ID equals mf.MASTER_FACILITY_ID select new { hm.MASTER_BLOCK_CODE, mf.IS_FINALIZED }).Where(c=>c.MASTER_BLOCK_CODE == item.MAST_BLOCK_CODE && (c.IS_FINALIZED == "N" || c.IS_FINALIZED == null)).Any()
                        ? "All Facilities are not finalized for the block"
                                : (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "N").Any() || !(dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any()))
                                    ? "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeFacilityBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                            
                                    : ""
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool FinalizeFacilityBlockPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_FACILITY_BLOCK_PMGSY3_FINALIZE FacilityPmgsy3Model = new MAST_FACILITY_BLOCK_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                FacilityPmgsy3Model.MAST_FACILITY_BLOCK_FIN_CODE = (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Any() ? dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Max(z => z.MAST_FACILITY_BLOCK_FIN_CODE) : 0) + 1;
                FacilityPmgsy3Model.MAST_BLOCK_CODE = blockCode;
                FacilityPmgsy3Model.MAST_FACILITY_BLOCK_FINALIZE_DATE = DateTime.Now;
                FacilityPmgsy3Model.IS_FINALIZED = "Y";

                FacilityPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                FacilityPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Add(FacilityPmgsy3Model);
                dbContext.SaveChanges();
                message = "Block finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("FinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeFacilityDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE FacilityPmgsy3Model = new MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                FacilityPmgsy3Model.MAST_FACILITY_DISTRICT_FIN_CODE = (dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any() ? dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Max(z => z.MAST_FACILITY_DISTRICT_FIN_CODE) : 0) + 1;
                FacilityPmgsy3Model.MAST_DISTRICT_CODE = districtCode;
                FacilityPmgsy3Model.MAST_FACILITY_DISTRICT_FINALIZE_DATE = DateTime.Now;
                FacilityPmgsy3Model.IS_FINALIZED = "Y";

                FacilityPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                FacilityPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Add(FacilityPmgsy3Model);
                dbContext.SaveChanges();
                message = "District finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("FinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region DeFinalize Facility PMGSY3 BLOCK/DISTRICT

        public bool DefinalizeFacilityDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG FacilityPmgsy3LogModel = new MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG();
                dbContext = new PMGSYEntities();

                MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE FacilityDetails = dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();

                using (TransactionScope ts = new TransactionScope())
                {
                    if (FacilityDetails != null)
                    {
                        FacilityPmgsy3LogModel.MAST_FACILITY_DISTRICT_FIN_CODE = (dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG.Any() ? dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG.Max(z => z.MAST_FACILITY_DISTRICT_FIN_CODE) : 0) + 1;
                        FacilityPmgsy3LogModel.MAST_DISTRICT_CODE = FacilityDetails.MAST_DISTRICT_CODE;
                        FacilityPmgsy3LogModel.MAST_FACILITY_DISTRICT_FINALIZE_DATE = FacilityDetails.MAST_FACILITY_DISTRICT_FINALIZE_DATE;
                        FacilityPmgsy3LogModel.IS_FINALIZED = "N";
                        FacilityPmgsy3LogModel.USERID = (FacilityDetails.USERID != null) ? FacilityDetails.USERID : null;
                        FacilityPmgsy3LogModel.IPADD = (FacilityDetails.IPADD != null) ? FacilityDetails.IPADD : null;

                        FacilityPmgsy3LogModel.UNLOCKED_BY_USERID = PMGSYSession.Current.UserId;
                        FacilityPmgsy3LogModel.UNLOCKED_BY_DATETIME = DateTime.Now;
                        FacilityPmgsy3LogModel.UNLOCKED_BY_IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG.Add(FacilityPmgsy3LogModel);
                        dbContext.SaveChanges();

                        //To delete finalized record after definalizing it
                        dbContext.Entry(FacilityDetails).State = EntityState.Deleted;
                        dbContext.SaveChanges();
                        message = "District Definalized successfully";
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        message = "No records found to definalize. District already definalized";
                        ts.Complete();
                        return false;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "DeFinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("DeFinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeFacilityDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeFacilityDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeFacilityDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DefinalizeFacilityBlockPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_FACILITY_BLOCK_PMGSY3_FINALIZE_LOG FacilityPmgsy3LogModel = new MAST_FACILITY_BLOCK_PMGSY3_FINALIZE_LOG();
                dbContext = new PMGSYEntities();

                MAST_FACILITY_BLOCK_PMGSY3_FINALIZE FacilityDetails = dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockCode).FirstOrDefault();

                using (TransactionScope ts = new TransactionScope())
                {
                    if (FacilityDetails != null)
                    {
                        FacilityPmgsy3LogModel.MAST_FACILITY_BLOCK_FIN_CODE = (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE_LOG.Any() ? dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE_LOG.Max(z => z.MAST_FACILITY_BLOCK_FIN_CODE) : 0) + 1;
                        FacilityPmgsy3LogModel.MAST_BLOCK_CODE = blockCode;
                        FacilityPmgsy3LogModel.MAST_FACILITY_BLOCK_FINALIZE_DATE = FacilityDetails.MAST_FACILITY_BLOCK_FINALIZE_DATE;
                        FacilityPmgsy3LogModel.IS_FINALIZED = "N";
                        FacilityPmgsy3LogModel.USERID = (FacilityDetails.USERID != null) ? FacilityDetails.USERID : null;
                        FacilityPmgsy3LogModel.IPADD = (FacilityDetails.IPADD != null) ? FacilityDetails.IPADD : null;

                        FacilityPmgsy3LogModel.UNLOCKED_BY_USERID = PMGSYSession.Current.UserId;
                        FacilityPmgsy3LogModel.UNLOCKED_BY_DATETIME = DateTime.Now;
                        FacilityPmgsy3LogModel.UNLOCKED_BY_IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE_LOG.Add(FacilityPmgsy3LogModel);
                        dbContext.SaveChanges();

                        dbContext.Entry(FacilityDetails).State = EntityState.Deleted;
                        dbContext.SaveChanges();
                        message = "Block Definalized successfully";
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        message = "No records found to definalize. Block already definalized";
                        ts.Complete();
                        return false;
                    }
                }

            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "DefinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("DefinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "DefinalizeFacilityBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "DefinalizeFacilityBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DefinalizeFacilityBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion
    }
}