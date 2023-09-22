
/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: MasterDataEntryDAL.cs

 * Author : Koustubh Nakate

 * Creation Date :06/Apr/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of master data entry screens.  
 * ---------------------------------------------------------------------------------------*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.MasterDataEntry;
using System.Data.Entity;
using PMGSY.Common;
using System.Transactions;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using System.IO;
//using System.Data.Entity.Core.SqlClient;
using System.Data.SqlClient;
using System.Data.Entity.Core;

namespace PMGSY.DAL
{

    #region Enums

    public enum MasterDataEntryModules
    {
        StateMaster = 1,
        DistrictMaster,
        BlockMaster,
        VillageMaster,
        HabitationMaster,
        PanchayatMaster,
        MLAConstituency,
        MPConstituency,
        MLABlocks,
        MPBlocks,
        PanchayatHabitation,
        RegionDistrict,
        TAAgencyStateDistrict,
        SRRDADistrict

    };

    public enum StateUTS
    {
        S = 1,
        U

    };
    public enum StateTypes
    {
        H = 1,//FOR NEW DATABASE
        //S=1,
        I,
        N,
        R,
        X,
        D
    };

    public enum MappingType
    {
        DistrictDetails = 1,
        RegionDistrict,
        AgencyDistrict,
        SRRDADistrict

    };

    #endregion 

    public class MasterDataEntryDAL : IMasterDataEntryDAL
    {
        //Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        #region StateMasterDataEntry
        /// <summary>
        /// This function is used to calculated max state code
        /// </summary>
        /// <param name="dbContext"> PMGSYEntities object</param>
        /// <returns> State Code</returns>
        public bool SaveStateDetailsDAL(MASTER_STATE master_state, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 recordCount = 0;

                recordCount = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_NAME.ToUpper() == master_state.MAST_STATE_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "State Name already exists.";
                    return false;
                }
                recordCount = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_SHORT_CODE.ToUpper() == master_state.MAST_STATE_SHORT_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "Short Name already exists.";
                    return false;
                }
                Models.MASTER_STATE stateDetails = new Models.MASTER_STATE();


                stateDetails.MAST_STATE_CODE = (Int32)GetMaxCode(MasterDataEntryModules.StateMaster); //GetMaxStateCode();
                stateDetails.MAST_STATE_NAME = master_state.MAST_STATE_NAME;
                stateDetails.MAST_STATE_UT = Enum.GetName(typeof(StateUTS), master_state.StateUTID); //== null ? null : master_state.MAST_STATE_UT;
                stateDetails.MAST_STATE_TYPE = Enum.GetName(typeof(StateTypes), master_state.StateTypeID); //master_state.MAST_STATE_TYPE;
                stateDetails.MAST_NIC_STATE_CODE = (Int32)master_state.MAST_NIC_STATE_CODE;
                stateDetails.MAST_STATE_SHORT_CODE = master_state.MAST_STATE_SHORT_NAME; //newly added column by deepak 4 Sept 2014
                stateDetails.MAST_LOCK_STATUS = "N";
                // stateDetails.DUMMY_STATE_CODE = master_state.DUMMY_STATE_CODE;

                //check for is active 
                stateDetails.MAST_STATE_ACTIVE = "Y";
                //modified by abhishek kamble 26-nov-2013
                stateDetails.USERID = PMGSYSession.Current.UserId;
                stateDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_STATE.Add(stateDetails);
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
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        public Int64 GetMaxCode(MasterDataEntryModules module)
        {
            Int64? maxCode = 0;
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                switch (module)
                {
                    case MasterDataEntryModules.StateMaster:
                        maxCode = (from stateMaster in dbContext.MASTER_STATE select (Int64?)stateMaster.MAST_STATE_CODE).Max();
                        break;

                    case MasterDataEntryModules.DistrictMaster:
                        maxCode = (from districtMaster in dbContext.MASTER_DISTRICT select (Int64?)districtMaster.MAST_DISTRICT_CODE).Max();
                        break;
                    case MasterDataEntryModules.BlockMaster:
                        maxCode = (from blockMaster in dbContext.MASTER_BLOCK select (Int64?)blockMaster.MAST_BLOCK_CODE).Max();
                        break;
                    case MasterDataEntryModules.VillageMaster:
                        maxCode = (from villageMaster in dbContext.MASTER_VILLAGE select (Int64?)villageMaster.MAST_VILLAGE_CODE).Max();
                        break;
                    case MasterDataEntryModules.HabitationMaster:
                        maxCode = (from habitationMaster in dbContext.MASTER_HABITATIONS select (Int64?)habitationMaster.MAST_HAB_CODE).Max();
                        break;
                    case MasterDataEntryModules.PanchayatMaster:
                        maxCode = (from panchayatMaster in dbContext.MASTER_PANCHAYAT select (Int64?)panchayatMaster.MAST_PANCHAYAT_CODE).Max();
                        break;
                    case MasterDataEntryModules.MLAConstituency:
                        maxCode = (from mlaconstituencyMaster in dbContext.MASTER_MLA_CONSTITUENCY select (Int64?)mlaconstituencyMaster.MAST_MLA_CONST_CODE).Max();
                        break;
                    case MasterDataEntryModules.MPConstituency:
                        maxCode = (from mpconstituencyMaster in dbContext.MASTER_MP_CONSTITUENCY select (Int64?)mpconstituencyMaster.MAST_MP_CONST_CODE).Max();
                        break;
                    case MasterDataEntryModules.MLABlocks:
                        maxCode = (from mlaBlocks in dbContext.MASTER_MLA_BLOCKS select (Int64?)mlaBlocks.MAST_MLA_BLOCK_ID).Max();
                        break;
                    case MasterDataEntryModules.MPBlocks:
                        maxCode = (from mpBlocks in dbContext.MASTER_MP_BLOCKS select (Int64?)mpBlocks.MAST_MP_BLOCK_ID).Max();
                        break;
                    case MasterDataEntryModules.PanchayatHabitation:
                        maxCode = (from panHab in dbContext.MASTER_PANCHAYAT_HABITATIONS select (Int64?)panHab.MAST_PAN_HAB_CODE).Max();
                        break;
                    case MasterDataEntryModules.RegionDistrict:
                        maxCode = (from regDist in dbContext.MASTER_REGION_DISTRICT_MAP select (Int64?)regDist.MAST_REGION_ID).Max();
                        break;
                    case MasterDataEntryModules.TAAgencyStateDistrict:
                        maxCode = (from TAStateDistrict in dbContext.ADMIN_TA_STATE select (Int64?)TAStateDistrict.ADMIN_TA_ID).Max();
                        break;
                    case MasterDataEntryModules.SRRDADistrict:
                        maxCode = (from agencyDistrict in dbContext.ADMIN_AGENCY_DISTRICT select (Int64?)agencyDistrict.ADMIN_AGENCY_DISTRICT_ID).Max();
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


        public MASTER_STATE GetStateDetailsDAL_ByStateCode(int stateCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Models.MASTER_STATE stateDetails = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).FirstOrDefault();

                MASTER_STATE master_state = null;
                if (stateDetails != null)
                {
                    master_state = new MASTER_STATE()
                    {
                        //MAST_STATE_CODE = stateDetails.MAST_STATE_CODE,
                        EncryptedStateCode = URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + stateDetails.MAST_STATE_CODE.ToString().Trim() }),
                        MAST_STATE_NAME = stateDetails.MAST_STATE_NAME.Trim(),
                        StateUTID = (byte)((StateUTS)(Enum.Parse(typeof(StateUTS), stateDetails.MAST_STATE_UT.Trim(), true))),  //0,
                        StateTypeID = (byte)((StateTypes)(Enum.Parse(typeof(StateTypes), stateDetails.MAST_STATE_TYPE.Trim(), true))),
                        MAST_NIC_STATE_CODE = stateDetails.MAST_NIC_STATE_CODE,
                        MAST_STATE_SHORT_NAME = stateDetails.MAST_STATE_SHORT_CODE == null ? "" : stateDetails.MAST_STATE_SHORT_CODE.ToString().Trim(),
                        // DUMMY_STATE_CODE = stateDetails.DUMMY_STATE_CODE
                    };
                }

                return master_state;

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



        public Array GetStateDetailsListDAL(bool isMap, int? page, int? rows, string sidx, string sord, out long totalRecords, int stateUT, int stateType)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                string strStateUT = stateUT == 0 ? string.Empty : (Enum.GetName(typeof(StateUTS), stateUT));
                string strStateType = stateType == 0 ? string.Empty : (Enum.GetName(typeof(StateTypes), stateType));
                //add check for isactive after database name change
                //List<Models.MASTER_STATE> lstStateDetails = (from master_State in dbContext.MASTER_STATE where master_State.MAST_STATE_ACTIVE=="Y" select master_State).ToList<Models.MASTER_STATE>();

                //for testing purpose stateType D and S are invalid records
                //List<Models.MASTER_STATE> lstStateDetails = (from master_State in dbContext.MASTER_STATE where master_State.MAST_STATE_ACTIVE == "Y" select master_State).ToList<Models.MASTER_STATE>();

                var lstStateDetails = from stateList in dbContext.MASTER_STATE
                                      where stateList.MAST_STATE_ACTIVE == "Y" &&
                                      (strStateUT == string.Empty ? "%" : stateList.MAST_STATE_UT) == (strStateUT == string.Empty ? "%" : strStateUT) &&
                                      (strStateType == string.Empty ? "%" : stateList.MAST_STATE_TYPE).StartsWith(strStateType == string.Empty ? "%" : strStateType)
                                      select new
                                      {
                                          stateList.MAST_STATE_NAME,
                                          stateList.MAST_STATE_CODE,
                                          stateList.MAST_STATE_TYPE,
                                          stateList.MAST_STATE_UT,
                                          stateList.MAST_STATE_SHORT_CODE,
                                          stateList.MAST_STATE_ACTIVE,
                                          stateList.MAST_NIC_STATE_CODE,
                                          stateList.MAST_LOCK_STATUS,
                                      };
                //Commented By Abhishek kamble to Display Disert State Type 29-Apr-2014
                //lstStateDetails = lstStateDetails.Where(s => s.MAST_STATE_TYPE != "D"); //|| s.MAST_STATE_TYPE != "S"

                //end for testing



                if (isMap)
                {
                    var mappedStates = (from agencyStates in dbContext.ADMIN_TA_STATE where agencyStates.MAST_IS_ACTIVE == "Y" && agencyStates.MAST_DISTRICT_CODE == null select new { agencyStates.MAST_STATE_CODE }).Distinct();
                    lstStateDetails = from stateList in lstStateDetails
                                      where !mappedStates.Any(stateCode => stateCode.MAST_STATE_CODE == stateList.MAST_STATE_CODE)
                                      select stateList;

                }

                //  IQueryable<Models.MASTER_STATE> query = lstStateDetails.AsQueryable<Models.MASTER_STATE>();

                var query = lstStateDetails;

                totalRecords = query.Count();

                if (rows != 0)
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {

                                case "StateName":
                                    query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "ShortName":
                                    query = query.OrderBy(x => x.MAST_STATE_SHORT_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateUT":
                                    query = query.OrderBy(x => x.MAST_STATE_UT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateType":
                                    query = query.OrderBy(x => x.MAST_STATE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "NICStateCode":
                                    query = query.OrderBy(x => x.MAST_NIC_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_STATE_UT).ThenBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                            }

                        }
                        else
                        {

                            switch (sidx)
                            {
                                case "StateName":
                                    query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "ShortName":
                                    query = query.OrderByDescending(x => x.MAST_STATE_SHORT_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateUT":
                                    query = query.OrderByDescending(x => x.MAST_STATE_UT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateType":
                                    query = query.OrderByDescending(x => x.MAST_STATE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "NICStateCode":
                                    query = query.OrderByDescending(x => x.MAST_NIC_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_STATE_UT).ThenBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "StateName":
                                    query = query.OrderBy(x => x.MAST_STATE_NAME);
                                    break;
                                case "ShortName":
                                    query = query.OrderBy(x => x.MAST_STATE_SHORT_CODE);
                                    break;
                                case "StateUT":
                                    query = query.OrderBy(x => x.MAST_STATE_UT);
                                    break;
                                case "StateType":
                                    query = query.OrderBy(x => x.MAST_STATE_TYPE);
                                    break;
                                case "NICStateCode":
                                    query = query.OrderBy(x => x.MAST_NIC_STATE_CODE);
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_STATE_UT).ThenBy(x => x.MAST_STATE_NAME);
                                    break;
                            }
                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "StateName":
                                    query = query.OrderByDescending(x => x.MAST_STATE_NAME);
                                    break;
                                case "ShortName":
                                    query = query.OrderByDescending(x => x.MAST_STATE_SHORT_CODE);
                                    break;
                                case "StateUT":
                                    query = query.OrderByDescending(x => x.MAST_STATE_UT);
                                    break;
                                case "StateType":
                                    query = query.OrderByDescending(x => x.MAST_STATE_TYPE);
                                    break;
                                case "NICStateCode":
                                    query = query.OrderByDescending(x => x.MAST_NIC_STATE_CODE);
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_STATE_NAME);
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_STATE_UT).ThenBy(x => x.MAST_STATE_NAME);
                    }
                }

                var result = query.Select(stateDetails => new
                {
                    stateDetails.MAST_STATE_CODE,
                    stateDetails.MAST_STATE_NAME,
                    stateDetails.MAST_STATE_UT,
                    stateDetails.MAST_STATE_TYPE,
                    stateDetails.MAST_NIC_STATE_CODE,
                    stateDetails.MAST_STATE_SHORT_CODE,
                    stateDetails.MAST_LOCK_STATUS
                }).ToArray();

                return result.Select(stateDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + stateDetails.MAST_STATE_CODE.ToString().Trim() }),
                    cell = new[] {                         
                                    stateDetails.MAST_STATE_NAME.Trim(),
                                    stateDetails.MAST_STATE_SHORT_CODE==null?string.Empty:stateDetails.MAST_STATE_SHORT_CODE.ToString().Trim(),
                                    StateUT.lstStateUT[stateDetails.MAST_STATE_UT.Trim()].ToString(),
                                    StateType.lstStateType[stateDetails.MAST_STATE_TYPE.Trim()].ToString(), 
                                    //stateDetails.DUMMY_STATE_CODE.ToString(),
                                    stateDetails.MAST_NIC_STATE_CODE.ToString().Trim(),//stateDetails.MAST_STATE_SHORT_CODE
                                    PMGSYSession.Current.RoleCode == 23 ? URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + stateDetails.MAST_STATE_CODE.ToString().Trim()}) : (stateDetails.MAST_LOCK_STATUS=="N"?URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + stateDetails.MAST_STATE_CODE.ToString().Trim()}): string.Empty)
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




        public bool UpdateStateDetailsDAL(MASTER_STATE master_state, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int stateCode = 0;
                Int32 recordCount = 0;
                encryptedParameters = master_state.EncryptedStateCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                stateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());

                recordCount = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_NAME.ToUpper() == master_state.MAST_STATE_NAME.ToUpper() && state.MAST_STATE_CODE != stateCode).Count();

                if (recordCount > 0)
                {
                    message = "State Name already exists.";
                    return false;
                }
                recordCount = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_SHORT_CODE.ToUpper() == master_state.MAST_STATE_SHORT_NAME.ToUpper() && state.MAST_STATE_CODE != stateCode).Count();

                if (recordCount > 0)
                {
                    message = "Short Name already exists.";
                    return false;
                }

                Models.MASTER_STATE stateDetails = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_CODE == stateCode).FirstOrDefault();


                stateDetails.MAST_STATE_NAME = master_state.MAST_STATE_NAME;
                stateDetails.MAST_STATE_UT = Enum.GetName(typeof(StateUTS), master_state.StateUTID);//master_state.MAST_STATE_UT; //== null ? null : master_state.MAST_STATE_UT;
                stateDetails.MAST_STATE_TYPE = Enum.GetName(typeof(StateTypes), master_state.StateTypeID); //master_state.MAST_STATE_TYPE;
                stateDetails.MAST_NIC_STATE_CODE = (Int32)master_state.MAST_NIC_STATE_CODE;
                stateDetails.MAST_STATE_SHORT_CODE = master_state.MAST_STATE_SHORT_NAME;
                //stateDetails.DUMMY_STATE_CODE = master_state.DUMMY_STATE_CODE;

                //modified by abhishek kamble 26-nov-2013
                stateDetails.USERID = PMGSYSession.Current.UserId;
                stateDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTER_ADDR"];

                dbContext.Entry(stateDetails).State = System.Data.Entity.EntityState.Modified;
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


        public bool DeleteStateDetailsDAL_ByStateCode(int stateCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_STATE master_state = dbContext.MASTER_STATE.Where(state => state.MAST_STATE_CODE == stateCode).FirstOrDefault();

                if (master_state == null)
                {
                    return false;
                }

                //modified by abhishek kamble 26-nov-2013
                master_state.USERID = PMGSYSession.Current.UserId;
                master_state.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(master_state).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_STATE.Remove(master_state);
                dbContext.SaveChanges();
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this State details.";
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

        public List<Models.MASTER_STATE> GetAllStates(bool flag)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                //return dbContext.MASTER_STATE.ToList<Models.MASTER_STATE>();
                List<Models.MASTER_STATE> stateList = new List<Models.MASTER_STATE>();
                stateList = (from state in dbContext.MASTER_STATE.Where(m => m.MAST_STATE_ACTIVE == "Y") orderby state.MAST_STATE_NAME select state).ToList<Models.MASTER_STATE>();

                if (flag)
                {
                    stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All States" });
                }

                return stateList;
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

        #endregion StateMasterDataEntry

        #region Addition of Facitlity
        /// <summary>
        /// added by abhinav pathak on 17-12-2018
        /// involve CRUD & filtering operations related to the facilities available in a district
        /// </summary>
        public bool DeleteFacilityDAL(int id)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.FACILITY_HABITATION_MAPPING mappingfacility = new PMGSY.Models.FACILITY_HABITATION_MAPPING();
                PMGSY.Models.MASTER_FACILITY masterFacility = new PMGSY.Models.MASTER_FACILITY();
                PMGSY.Models.FACILITY_IMAGE_DELETION LogModel = new Models.FACILITY_IMAGE_DELETION();

                using (TransactionScope ts = new TransactionScope())
                {
                    var LogDetailsList = dbContext.FACILITY_IMAGE_DELETION.Where(x => x.MAST_FACILITY_ID == id).ToList();

                    if (LogDetailsList.Count != 0)
                    {
                        foreach (var item in LogDetailsList)
                        {
                            dbContext.FACILITY_IMAGE_DELETION.Remove(item);
                        }
                    }

                    masterFacility = (from item in dbContext.MASTER_FACILITY
                                      where item.MASTER_FACILITY_ID == id
                                      select item).FirstOrDefault();
                    mappingfacility = (from item in dbContext.FACILITY_HABITATION_MAPPING
                                       where item.MASTER_FACILITY_ID == id
                                       select item).FirstOrDefault();

                    dbContext.FACILITY_HABITATION_MAPPING.Remove(mappingfacility);

                    dbContext.MASTER_FACILITY.Remove(masterFacility);

                    dbContext.SaveChanges();
                    ts.Complete();
                }

                if (masterFacility.FILE_NAME == null)
                {
                    return true;
                }


                if (File.Exists(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], masterFacility.FILE_NAME != null ? masterFacility.FILE_NAME : " ")))
                {
                    //File.Delete(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] + @"\" + masterFacility.FILE_NAME);
                    //File.Delete(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_THUMBNAIL"] + @"\" + masterFacility.FILE_NAME);

                    File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] , masterFacility.FILE_NAME),Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_BACKUP"] , masterFacility.FILE_NAME));
                    File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_THUMBNAIL"] , masterFacility.FILE_NAME), Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_THUMBNAIL_BACKUP"] , masterFacility.FILE_NAME));
                    return true;
                }

                return false;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDAl.DeleteFacilityDAL()");
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

        public string GetFacilityCategory(Int32 MASTER_FACILITY_CATEGORY_ID)
        {
            PMGSY.Models.PMGSYEntities dbContext = null;
            string MASTER_FACILITY_CATEGORY_NAME = string.Empty;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                MASTER_FACILITY_CATEGORY_NAME = dbContext.MASTER_FACILITY_CATEGORY.Where(x => x.MASTER_FACILITY_CATEGORT_ID == MASTER_FACILITY_CATEGORY_ID).Select(x => x.MASTER_FACILITY_CATEGORY_NAME).FirstOrDefault();
                if (string.IsNullOrEmpty(MASTER_FACILITY_CATEGORY_NAME))
                {
                    return MASTER_FACILITY_CATEGORY_NAME = "-";
                }
                else
                {
                    return MASTER_FACILITY_CATEGORY_NAME;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDAl.GetFacilityCategory()");
                return string.Empty;

            }
        }

        public Array GetFacilityDetailsListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                //DateTime date = new DateTime(2019-08-26 00:00:00.000)
                DateTime dateToCheck = new DateTime(2019, 08, 27);
                int facilitycode = Convert.ToInt32(ModelParam.ElementAt(3));
                int facilityType = Convert.ToInt32(ModelParam.ElementAt(4));
                int blockcode = Convert.ToInt32(ModelParam.ElementAt(5));
                int habitationcode = Convert.ToInt32(ModelParam.ElementAt(6));

                var FacilityList = (from item in dbContext.MASTER_FACILITY

                                    join factID in dbContext.MASTER_FACILITY_CATEGORY on
                                    item.MASTER_FACILITY_SUB_CATEGORY_ID equals factID.MASTER_FACILITY_CATEGORT_ID

                                    join habcode in dbContext.FACILITY_HABITATION_MAPPING on
                                    item.MASTER_FACILITY_ID equals habcode.MASTER_FACILITY_ID

                                    join habname in dbContext.MASTER_HABITATIONS on
                                    habcode.MASTER_HAB_CODE equals habname.MAST_HAB_CODE

                                    join blockname in dbContext.MASTER_BLOCK on
                                    habcode.MASTER_BLOCK_CODE equals blockname.MAST_BLOCK_CODE

                                    join districtname in dbContext.MASTER_DISTRICT on
                                    habcode.MASTER_DISTRICT_CODE equals districtname.MAST_DISTRICT_CODE

                                    where
                                        //facilitycode == 0 ? true : factID.MASTER_FACILITY_PARENT_ID == facilitycode
                                    factID.MASTER_FACILITY_PARENT_ID == (facilitycode == 0 ? factID.MASTER_FACILITY_PARENT_ID : facilitycode)
                                    &&
                                        //facilityType == 0 ? true : item.MASTER_FACILITY_SUB_CATEGORY_ID == facilityType
                                    item.MASTER_FACILITY_SUB_CATEGORY_ID == (facilityType == 0 ? item.MASTER_FACILITY_SUB_CATEGORY_ID : facilityType)
                                    &&
                                        //blockcode == 0 ? true : blockname.MAST_BLOCK_CODE == blockcode
                                    blockname.MAST_BLOCK_CODE == (blockcode == 0 ? blockname.MAST_BLOCK_CODE : blockcode)
                                    &&
                                        //habitationcode == 0 ? true : habcode.MASTER_HAB_CODE == habitationcode
                                    habcode.MASTER_HAB_CODE == (habitationcode == 0 ? habcode.MASTER_HAB_CODE : habitationcode)
                                    &&
                                    habcode.MASTER_DISTRICT_CODE == PMGSYSession.Current.DistrictCode
                                    select new
                                    {
                                        item.MASTER_FACILITY_ID,
                                        factID.MASTER_FACILITY_CATEGORY_NAME,
                                        item.MASTER_FACILITY_DESC,
                                        item.MASTER_FACILITY_SUB_CATEGORY_ID,
                                        item.ADDRESS,
                                        item.MASTER_FACILITY_CATEGORY_ID,
                                        item.PINCODE,
                                        habcode.MASTER_HAB_CODE,
                                        habname.MAST_HAB_NAME,
                                        blockname.MAST_BLOCK_NAME,
                                        districtname.MAST_DISTRICT_NAME,
                                        districtname.MAST_DISTRICT_CODE,
                                        habcode.MASTER_BLOCK_CODE,
                                        factID.MASTER_FACILITY_CATEGORT_ID,
                                        item.FILE_NAME,
                                        item.FILE_UPLOAD_DATE,
                                        item.LATITUDE,
                                        item.LONGITUDE,
                                        item.IS_FINALIZED
                                    }).ToList();

                var habName = (from item in dbContext.FACILITY_HABITATION_MAPPING
                               join hab in dbContext.MASTER_HABITATIONS on item.MASTER_HAB_CODE equals hab.MAST_HAB_CODE

                               select new
                               {
                                   hab.MAST_HAB_CODE,
                                   hab.MAST_HAB_NAME
                               }).ToList();

                //FacilityList = FacilityList.Where(x => x.MAST_DISTRICT_NAME == PMGSYSession.Current.DistrictName && x.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode
                //    //&& blockcode > 0 ? x.MASTER_BLOCK_CODE == blockcode : true
                //     //&& x.MASTER_BLOCK_CODE == (blockcode > 0 ? blockcode : x.MASTER_BLOCK_CODE)
                //     //&& x.MASTER_FACILITY_CATEGORY_ID == (facilitycode > 0 ? facilitycode : x.MASTER_FACILITY_CATEGORY_ID)
                //     //&& x.MASTER_FACILITY_SUB_CATEGORY_ID == (facilityType > 0 ? facilityType : x.MASTER_FACILITY_SUB_CATEGORY_ID)
                //     //&& x.MASTER_HAB_CODE == (habitationcode > 0 ? habitationcode : x.MASTER_HAB_CODE)
                //    ).ToList();

                totalRecords = FacilityList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "FacilityID":
                                FacilityList = FacilityList.OrderBy(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "FacilityID":
                                FacilityList = FacilityList.OrderByDescending(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    FacilityList = FacilityList.OrderBy(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var facility = FacilityList.Select(facilityDetails => new
                {
                    facilityDetails.MASTER_FACILITY_ID,
                    facilityDetails.MASTER_FACILITY_CATEGORY_ID,
                    facilityDetails.MAST_HAB_NAME,
                    facilityDetails.MASTER_FACILITY_DESC,
                    facilityDetails.MASTER_FACILITY_CATEGORY_NAME,
                    facilityDetails.ADDRESS,
                    facilityDetails.PINCODE,
                    facilityDetails.MAST_DISTRICT_NAME,
                    facilityDetails.MAST_BLOCK_NAME,
                    facilityDetails.FILE_NAME,
                    facilityDetails.FILE_UPLOAD_DATE,
                    facilityDetails.LATITUDE,
                    facilityDetails.LONGITUDE,
                    facilityDetails.IS_FINALIZED
                }).ToArray();

                return facility.Select(fac => new
                {
                    cell = new[]
                {
                 
                    URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +  fac.MASTER_FACILITY_ID.ToString()}),
                    fac.MASTER_FACILITY_CATEGORY_ID.ToString(),
                    fac.MAST_DISTRICT_NAME,
                    fac.MAST_BLOCK_NAME,
                    fac.MAST_HAB_NAME.ToString(),
                    GetFacilityCategory(fac.MASTER_FACILITY_CATEGORY_ID),
                    fac.MASTER_FACILITY_CATEGORY_NAME.ToString(),
                    fac.MASTER_FACILITY_DESC.ToString(),
                    fac.ADDRESS.ToString(),
                    fac.PINCODE.ToString(),
                    fac.FILE_NAME != null ? Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_VIRTUAL_DIR_PATH"] , fac.FILE_NAME) + "#" + 
                    (fac.FILE_NAME == null  ? " " :
                    
                    (fac.IS_FINALIZED=="Y"  || fac.FILE_UPLOAD_DATE > dateToCheck
                    
                    && (File.Exists(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] , fac.FILE_NAME))?
                       (Nullable<long>) new FileInfo(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] , fac.FILE_NAME)).Length != 0 : false)

                    ? " " : 
                    
                    "<input type='button' value='Delete Photograph' class='jqueryButton' title='click here to delete image' onClick = DeleteImageFromGrid('" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +fac.MASTER_FACILITY_ID.ToString()}) + "') />")) : " ",


                    // Following is added on 25 June 2020
                    (fac.LATITUDE==null||fac.LONGITUDE==null||fac.FILE_NAME==null)
                   
                   ? "No": (fac.IS_FINALIZED == null ? "No" : (fac.IS_FINALIZED.Equals("Y") ? "Yes" : "No")),

                    // Following is commented on 25 June 2020
                   //(fac.LATITUDE==null||fac.LONGITUDE==null||fac.FILE_NAME==null)
                   
                   //? "-": (fac.IS_FINALIZED == null ? "No" : (fac.IS_FINALIZED.Equals("Y") ? "Yes" : "No")),// 
                    
                    fac.FILE_NAME == null && (fac.LATITUDE != null || fac.LONGITUDE != null)?
                    "<center><span class='ui-icon ui-icon-image' title='Click here to upload photograph' onClick ='UploadFacilityPhotoGraph(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +    fac.MASTER_FACILITY_ID.ToString()}) + 
                    "\");'></span></center>"
                    : "-",

                 //   fac.IS_FINALIZED==null?"-":(fac.IS_FINALIZED=="Y"?"<center><span class='ui-icon ui-icon-locked' title='Click here to definalize facility' onClick ='definalizeFacility(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +fac.MASTER_FACILITY_ID.ToString()})+ "\");'></span></center>":"<center><span class='ui-icon ui-icon-unlocked' title='Facility is definalized' onClick ='noAction(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +fac.MASTER_FACILITY_ID.ToString()})+ "\");'></span></center>")

                    //"<input type='button' value='Delete Image' class='jqueryButton' title='click here to delete image' onClick = DeleteImageFromGrid('" + fac.MASTER_FACILITY_ID + "') />")) : " ",
                    
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "MasterDataEntryDAl.GetFacilityDetailsListDAL()");
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

        //public bool SaveFacilityDetailsDAL(CreateFacility createFacility, ref string message)
        //{
        //    PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
        //    try
        //    {
        //        //using (TransactionScope ts = new TransactionScope())
        //        //{

        //            PMGSY.Models.MASTER_FACILITY masterFacility = new PMGSY.Models.MASTER_FACILITY();
        //            PMGSY.Models.FACILITY_HABITATION_MAPPING mappingfacility = new PMGSY.Models.FACILITY_HABITATION_MAPPING();

        //            //Avinash
        //            //Description Validation
        //            if (dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == createFacility.MAST_DISTRICT_CODE && x.MASTER_BLOCK_CODE == createFacility.MAST_BLOCK_CODE && x.MASTER_HAB_CODE == createFacility.HabitationCode).Any())
        //            {
        //                List<PMGSY.Models.FACILITY_HABITATION_MAPPING> lstMapping = new List<PMGSY.Models.FACILITY_HABITATION_MAPPING>();
        //                PMGSY.Models.MASTER_FACILITY master = new PMGSY.Models.MASTER_FACILITY();
        //                lstMapping = dbContext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_DISTRICT_CODE == createFacility.MAST_DISTRICT_CODE && x.MASTER_BLOCK_CODE == createFacility.MAST_BLOCK_CODE && x.MASTER_HAB_CODE == createFacility.HabitationCode).ToList();
        //                foreach (var item in lstMapping)
        //                {
        //                    master = dbContext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == item.MASTER_FACILITY_ID && x.MASTER_FACILITY_CATEGORY_ID == createFacility.facilityCode && x.MASTER_FACILITY_SUB_CATEGORY_ID == createFacility.FacilityName).FirstOrDefault();

        //                    if (master != null)
        //                    {
        //                        if (master.MASTER_FACILITY_DESC.ToUpper() == (createFacility.FacilityDescription.ToUpper()) || master.MASTER_FACILITY_DESC.ToLower() == createFacility.FacilityDescription.ToLower())
        //                        {
        //                            message = "Facility Name already exists in Habitation. Same facility can only be entered against one habitation. Incase it is a different facility please give an unique name.";
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }


        //            if (createFacility.FacilityDescription.Length > 200)
        //            {
        //                message = "Maximum 200 Charcters allowed in Facility Description";
        //                return false;
        //            }


        //            if (createFacility.address.Length > 255)
        //            {
        //                message = "Maximum 255 Charcters allowed in Address";
        //                return false;
        //            }


        //            if (createFacility.facilityCode == 0)
        //            {
        //                message = "Please Select Facility Category";
        //                return false;
        //            }

        //            if (createFacility.FacilityName == 0)
        //            {
        //                message = "Please Select Facility Type";
        //                return false;
        //            }

        //            if (string.IsNullOrEmpty(createFacility.FacilityDescription))
        //            {
        //                message = "Please Enter Facility Description";
        //                return false;
        //            }


        //            if (string.IsNullOrEmpty(createFacility.address))
        //            {
        //                message = "Please Enter Address";
        //                return false;
        //            }

        //            if (string.IsNullOrEmpty(createFacility.pincode))
        //            {
        //                message = "Please Enter PinCode";
        //                return false;
        //            }


        //            if (createFacility.MAST_BLOCK_CODE == 0)
        //            {
        //                message = "Please Select Block";
        //                return false;
        //            }


        //            if (createFacility.HabitationCode == 0)
        //            {
        //                message = "Please Select Habitation";
        //                return false;
        //            }

        //            int maxcolumn = 0;
        //            int MappingColMax = 0;
        //            var a = (from item in dbContext.MASTER_FACILITY
        //                     select item.MASTER_FACILITY_ID).ToList();


        //            var facilityMappingMax = (from item in dbContext.FACILITY_HABITATION_MAPPING
        //                                      select item.ID).ToList();

        //            if (facilityMappingMax.Count != 0)
        //            {
        //                MappingColMax = facilityMappingMax.Max();
        //            }

        //            if (a.Count != 0)
        //            {
        //                maxcolumn = a.Max();
        //            }


        //                int maxFacilityID = (maxcolumn == 0 ? 1 : (maxcolumn + 1));
        //                masterFacility.MASTER_FACILITY_ID = maxFacilityID;
        //                masterFacility.MASTER_FACILITY_CATEGORY_ID = createFacility.facilityCode;
        //                masterFacility.MASTER_FACILITY_SUB_CATEGORY_ID = createFacility.FacilityName;
        //                masterFacility.ADDRESS = createFacility.address.Trim();
        //                masterFacility.MASTER_FACILITY_DESC = createFacility.FacilityDescription.Trim();
        //                masterFacility.PINCODE = Convert.ToInt32(createFacility.pincode);
        //                masterFacility.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //                masterFacility.USERID = PMGSYSession.Current.UserId;

        //                mappingfacility.ID = (MappingColMax == 0 ? 1 : (MappingColMax + 1));
        //                mappingfacility.MASTER_DISTRICT_CODE = createFacility.MAST_DISTRICT_CODE;
        //                mappingfacility.MASTER_BLOCK_CODE = createFacility.MAST_BLOCK_CODE;
        //                mappingfacility.MASTER_HAB_CODE = createFacility.HabitationCode;
        //                mappingfacility.MASTER_FACILITY_ID = maxFacilityID;
        //                mappingfacility.USERID = PMGSYSession.Current.UserId;
        //                mappingfacility.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //                dbContext.FACILITY_HABITATION_MAPPING.Add(mappingfacility);
        //                dbContext.MASTER_FACILITY.Add(masterFacility);
        //                dbContext.SaveChanges();
        //                message = "Facility Details Saved Successfully";

        //                return true;

        //       //}
        //    } // try 

        //    catch (Exception ex)
        //    {
        //        message = "Facility Details Could Not be Saved";
        //        ErrorLog.LogError(ex, "MasterDataEntryDAl.SaveFacilityDetailsDAL()");
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}

        public bool SaveFacilityDetailsDAL(CreateFacility createFacility, ref string message)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_FACILITY masterFacility = new PMGSY.Models.MASTER_FACILITY();
                PMGSY.Models.FACILITY_HABITATION_MAPPING mappingfacility = new PMGSY.Models.FACILITY_HABITATION_MAPPING();

                //Avinash
                //Description Validation
                if (createFacility.FacilityDescription.Length > 200)
                {
                    message = "Maximum 200 Charcters allowed in Facility Description";
                    return false;
                }

                if (createFacility.address.Length > 255)
                {
                    message = "Maximum 255 Charcters allowed in Address";
                    return false;
                }

                if (createFacility.facilityCode == 0)
                {
                    message = "Please Select Facility Category";
                    return false;
                }

                if (createFacility.FacilityName == 0)
                {
                    message = "Please Select Facility Type";
                    return false;
                }

                if (string.IsNullOrEmpty(createFacility.FacilityDescription))
                {
                    message = "Please Enter Facility Description";
                    return false;
                }

                if (string.IsNullOrEmpty(createFacility.address))
                {
                    message = "Please Enter Address";
                    return false;
                }

                if (string.IsNullOrEmpty(createFacility.pincode))
                {
                    message = "Please Enter PinCode";
                    return false;
                }


                if (createFacility.MAST_BLOCK_CODE == 0)
                {
                    message = "Please Select Block";
                    return false;
                }


                if (createFacility.HabitationCode == 0)
                {
                    message = "Please Select Habitation";
                    return false;
                }

                int pin = Convert.ToInt32(createFacility.pincode);

                int? result = dbContext.USP_facility_insert_facility_details(createFacility.facilityCode, createFacility.FacilityName, createFacility.FacilityDescription.Trim(),
                    createFacility.address, pin, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], createFacility.MAST_DISTRICT_CODE, createFacility.MAST_BLOCK_CODE,
                    createFacility.HabitationCode, null, null, null).FirstOrDefault();

                if (result == 1)
                {
                    message = "Facility details saved successfully";
                }
                if (result == -1)
                {
                    message = "Facility Name already exists in Habitation. Same facility can only be entered against one habitation. Incase it is a different facility please give an unique name.";
                }

                return true;

            } // try 

            catch (Exception ex)
            {
                message = "Facility Details Could Not be Saved";
                ErrorLog.LogError(ex, "MasterDataEntryDAl.SaveFacilityDetailsDAL()");
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

        public CreateFacility DisplayfacilityDetailsDAL(int facilityID)
        {
            CreateFacility model = new CreateFacility();
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

                var facilityInfo = (from item in dbContext.MASTER_FACILITY
                                    where item.MASTER_FACILITY_ID == facilityID
                                    select item).FirstOrDefault();

                var facilitycategory = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                        where item.MASTER_FACILITY_CATEGORT_ID == facilityInfo.MASTER_FACILITY_SUB_CATEGORY_ID
                                        select item).FirstOrDefault();


                var facilityParentCategory = (from item in dbContext.MASTER_FACILITY_CATEGORY
                                              where item.MASTER_FACILITY_CATEGORT_ID == facilityInfo.MASTER_FACILITY_CATEGORY_ID
                                              select item).FirstOrDefault();

                var habitationDetails = (from item in dbContext.FACILITY_HABITATION_MAPPING
                                         where item.MASTER_FACILITY_ID == facilityID
                                         select item).FirstOrDefault();

                var districtname = (from item in dbContext.MASTER_DISTRICT
                                    where item.MAST_DISTRICT_CODE == habitationDetails.MASTER_DISTRICT_CODE
                                    select item).FirstOrDefault();

                var BlockName = (from item in dbContext.MASTER_BLOCK
                                 where item.MAST_BLOCK_CODE == habitationDetails.MASTER_BLOCK_CODE
                                 select item).FirstOrDefault();

                var HabName = (from item in dbContext.MASTER_HABITATIONS
                               where item.MAST_HAB_CODE == habitationDetails.MASTER_HAB_CODE
                               select item).FirstOrDefault();


                // Added by Rohit on 4 Sept 2019
                if (facilityInfo.FINALIZED_DATE != null)
                {
                    System.DateTime? date = facilityInfo.FINALIZED_DATE;
                    string Date = Convert.ToString(date);
                    model.FinalizedDate = (facilityInfo.FINALIZED_DATE != null ? Date.Remove(10, 9) : "-");
                }
                else
                {
                    model.FinalizedDate = "-";
                }

                model.IsFinalized = (facilityInfo.IS_FINALIZED == null ? "No" : (facilityInfo.IS_FINALIZED.Equals("Y") ? "Yes" : "No"));
                //

                model.FacilityID = facilityInfo.MASTER_FACILITY_ID;
                model.DistrictName = districtname.MAST_DISTRICT_NAME;
                model.blockName = BlockName.MAST_BLOCK_NAME;
                model.habName = HabName.MAST_HAB_NAME;
                model.FacilityCategory = facilitycategory.MASTER_FACILITY_CATEGORY_NAME;
                model.FacilityParentCategory = facilityParentCategory.MASTER_FACILITY_CATEGORY_NAME;
                model.DisplayAddress = facilityInfo.ADDRESS;
                model.DisplayPIN = Convert.ToString(facilityInfo.PINCODE);
                model.FacilityDesc = facilityInfo.MASTER_FACILITY_DESC;
                model.Latitude = Convert.ToDouble(facilityInfo.LATITUDE);
                model.Longitude = Convert.ToDouble(facilityInfo.LONGITUDE);
                model.UploadDate = Convert.ToString(facilityInfo.FILE_UPLOAD_DATE);
                model.FacilityID = facilityID;
                model.FileName = facilityInfo.FILE_NAME;
                string FACILITY_PHYSICAL_LOCATION = ConfigurationManager.AppSettings["FACILITY_PHYSICAL_LOCATION"].ToString();
                string FACILITY_VIRTUAL_LOCATION = ConfigurationManager.AppSettings["FACILITY_VIRTUAL_LOCATION"].ToString();

                //if (facilityInfo.FILE_NAME != null)
                //{

                //    string PhysicalFilePath = FACILITY_PHYSICAL_LOCATION + "\\" + facilityInfo.FILE_NAME;
                //    string VirtualFilePath = FACILITY_VIRTUAL_LOCATION + "/" + "thumbnails" + "/" + facilityInfo.FILE_NAME;

                //    if (File.Exists(PhysicalFilePath))  //Checking Physical File Path
                //    {
                //        if (File.Exists(VirtualFilePath))  //Checking  File Path
                //        {
                //            model.FileName = VirtualFilePath;
                //        }
                //        else
                //        {
                //            model.FileName = string.Empty;
                //        }
                //    }
                //    else
                //    {
                //        model.FileName = string.Empty;

                //    }
                //}
                //else
                //{
                //    model.FileName = string.Empty;
                //}

                return model;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDAL/DisplayfacilityDetailsDAL");
                return model;
            }

        }

        public bool DeleteImageLatLongDAL(int facilityID, string remark)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new Models.PMGSYEntities();
            try
            {
                bool isupdated = false;
                var model = dbcontext.MASTER_FACILITY.Where(obj => obj.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                if (model != null)
                {
                    string filename = model.FILE_NAME.Trim();
                    if (File.Exists(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], filename)) || filename !=null)
                    {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            PMGSY.Models.FACILITY_IMAGE_DELETION FACILITY_IMAGE_DELETION = new Models.FACILITY_IMAGE_DELETION();
                            model.FILE_NAME = null;

                            dbcontext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            int? MaxID = null;

                            if (dbcontext.FACILITY_IMAGE_DELETION.Count() == 0)
                            {
                                MaxID = 1;
                            }
                            else
                            {
                                MaxID = dbcontext.FACILITY_IMAGE_DELETION.Select(x => x.ID).Max() + 1;
                            }
                            FACILITY_IMAGE_DELETION.ID = Convert.ToInt32(MaxID);
                            FACILITY_IMAGE_DELETION.MAST_FACILITY_ID = facilityID;
                            FACILITY_IMAGE_DELETION.USERID = PMGSYSession.Current.UserId;
                            FACILITY_IMAGE_DELETION.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            FACILITY_IMAGE_DELETION.FILE_DELETION_REMARKS = remark;
                            FACILITY_IMAGE_DELETION.FILE_DELETION_DATE = DateTime.Now;
                            FACILITY_IMAGE_DELETION.FILE_NAME = filename.Trim();

                            dbcontext.FACILITY_IMAGE_DELETION.Add(FACILITY_IMAGE_DELETION);

                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("FACILITY_FILE_UPLOAD : " + "Application_Error()");

                                sw.WriteLine("Path : " + Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], filename));
                                sw.WriteLine("Main image Backup" + Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_BACKUP"], filename));
                                sw.WriteLine("Key" + ConfigurationManager.AppSettings["FACILITY_IMAGE_BACKUP"]);
                                sw.WriteLine("Value" + filename);
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }

                            if (File.Exists(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], filename)))
                            {
                                File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], filename), Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_BACKUP"], filename));
                                File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_THUMBNAIL"], filename), Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_THUMBNAIL_BACKUP"], filename));
                            }

                           // File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], filename), Path.Combine(ConfigurationManager.AppSettings["FACILITY_IMAGE_BACKUP"] , filename));
                            //File.Move(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_THUMBNAIL"] , filename), Path.Combine( ConfigurationManager.AppSettings["FACILITY_IMAGE_THUMBNAIL_BACKUP"] , filename));

                            dbcontext.SaveChanges();
                            ts.Complete();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDal/DeleteImageLatLong");
                return false;
            }
        }

        public bool SavePhotoGraphDAL(int facilityID, string FileName, HttpPostedFileBase filebase)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new Models.PMGSYEntities();
            try
            {
                var dbmodel = dbcontext.MASTER_FACILITY.Where(x => x.MASTER_FACILITY_ID == facilityID).FirstOrDefault();
                var HabID = dbcontext.FACILITY_HABITATION_MAPPING.Where(x => x.MASTER_FACILITY_ID == dbmodel.MASTER_FACILITY_ID).FirstOrDefault().MASTER_HAB_CODE;
                if (dbmodel != null)
                {

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    FileName = "IMG_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + HabID.ToString() + ".jpeg";
                    using (TransactionScope ts = new TransactionScope())
                    {
                        dbmodel.FILE_NAME = FileName;
                        dbcontext.Entry(dbmodel).State = System.Data.Entity.EntityState.Modified;

                        dbcontext.SaveChanges();
                        ts.Complete();
                    }
                    filebase.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"], FileName));
                    filebase.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_THUMBNAIL"], FileName));
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterEntryDAL/SavePhotoGraph");
                return false;
            }
        }

        #endregion

        #region DistrictMasterDataEntry
        public bool SaveDistrictDetailsDAL(MASTER_DISTRICT master_district, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Int32 recordCount = dbContext.MASTER_DISTRICT.Where(district => district.MAST_STATE_CODE == master_district.MAST_STATE_CODE && district.MAST_DISTRICT_NAME.ToUpper() == master_district.MAST_DISTRICT_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "District Name under selected state is already exist.";
                    return false;
                }
                Models.MASTER_DISTRICT districtDetails = new Models.MASTER_DISTRICT();

                //added by Ujjwal Saket on 23-10-2013
                //Int32? maxDistrict = 1;
                //if (dbContext.MASTER_DISTRICT.Any(m => m.MAST_STATE_CODE == master_district.MAST_STATE_CODE))
                //{
                //    maxDistrict = (from district in dbContext.MASTER_DISTRICT where district.MAST_STATE_CODE == master_district.MAST_STATE_CODE select district.MAST_DISTRICT_ID).Max() + 1;
                //}

                //finish addition
                int maxDistrictID = dbContext.MASTER_DISTRICT.Where(a => a.MAST_STATE_CODE == master_district.MAST_STATE_CODE).Max(a => (Int32?)a.MAST_DISTRICT_ID) == null ? 1 : (Int32)dbContext.MASTER_DISTRICT.Where(a => a.MAST_STATE_CODE == master_district.MAST_STATE_CODE).Max(a => (Int32?)a.MAST_DISTRICT_ID) + 1; //new change by deepak 4sept2014

                districtDetails.MAST_DISTRICT_CODE = (Int32)GetMaxCode(MasterDataEntryModules.DistrictMaster);
                districtDetails.MAST_DISTRICT_ID = maxDistrictID;
                districtDetails.MAST_DISTRICT_NAME = master_district.MAST_DISTRICT_NAME;
                districtDetails.MAST_STATE_CODE = master_district.MAST_STATE_CODE; //== null ? null : master_state.MAST_STATE_UT;
                districtDetails.MAST_PMGSY_INCLUDED = master_district.IsPMGSYIncluded == true ? "Y" : "N";
                districtDetails.MAST_IAP_DISTRICT = master_district.IsIAPDistrict == true ? "Y" : "N";
                districtDetails.MAST_DISTRICT_ACTIVE = "Y";
                Int32? NICStateCode = (from state in dbContext.MASTER_STATE where state.MAST_STATE_CODE == master_district.MAST_STATE_CODE select (Int32)state.MAST_NIC_STATE_CODE).FirstOrDefault();
                districtDetails.MAST_NIC_STATE_CODE = NICStateCode == null ? 0 : (Int32)NICStateCode;
                districtDetails.MAST_LOCK_STATUS = "N";
                // districtDetails.MAST_DISTRICT_ID = maxDistrict.Value;


                //added by abhishek kamble 26-nov-2013         
                districtDetails.USERID = PMGSYSession.Current.UserId;
                districtDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_DISTRICT.Add(districtDetails);
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


        public Array GetDistrictDetailsListDAL(int agencyCode, int regionCode, int adminNdCode, bool isMap, MappingType mapping, int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {


                var query = from districtDetails in dbContext.MASTER_DISTRICT
                            join stateDetails in dbContext.MASTER_STATE
                            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where// districtDetails.MAST_DISTRICT_ACTIVE == "Y" &&
                                //stateDetails.MAST_STATE_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                            select new
                            {
                                districtDetails.MAST_DISTRICT_CODE,
                                districtDetails.MAST_DISTRICT_NAME,
                                stateDetails.MAST_STATE_NAME,
                                districtDetails.MAST_PMGSY_INCLUDED,
                                districtDetails.MAST_IAP_DISTRICT,
                                districtDetails.MAST_STATE_CODE,
                                districtDetails.MAST_DISTRICT_ID,
                                districtDetails.MAST_DISTRICT_ACTIVE,
                                MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(districtDetails.MAST_STATE_CODE, districtDetails.MAST_DISTRICT_CODE, 0, 0, 0, 0, 0, 0, "DM", 1, (short)PMGSYSession.Current.RoleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "M" : districtDetails.MAST_LOCK_STATUS)
                            };
                //else if (stateCode > 0)
                //{

                //   query = from districtDetails in dbContext.MASTER_DISTRICT
                //            join stateDetails in dbContext.MASTER_STATE
                //            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                //            where districtDetails.MAST_DISTRICT_ACTIVE == "Y" && districtDetails.MAST_STATE_CODE==stateCode
                //            select new { districtDetails.MAST_DISTRICT_CODE, districtDetails.MAST_DISTRICT_NAME, stateDetails.MAST_STATE_NAME, districtDetails.MAST_PMGSY_INCLUDED, districtDetails.MAST_IAP_DISTRICT };
                //}


                if (isMap)
                {
                    switch (mapping)
                    {
                        case MappingType.RegionDistrict:
                            var mappedDistricts = (from regionDistrictDetails in dbContext.MASTER_REGION_DISTRICT_MAP where regionDistrictDetails.MAST_REGION_CODE == regionCode && regionDistrictDetails.MAST_REGION_DISTRICT_ACTIVE == "Y" select new { regionDistrictDetails.MAST_DISTRICT_CODE }).Distinct(); //change by deepak on 01/10/2014
                            query = from districtList in query
                                    where !mappedDistricts.Any(districtCode => districtCode.MAST_DISTRICT_CODE == districtList.MAST_DISTRICT_CODE)
                                    select districtList;
                            break;
                        case MappingType.AgencyDistrict:
                            //var mappedDistricts_TA = (from agencyDistrictDetails in dbContext.ADMIN_TA_STATE select new { agencyDistrictDetails.MAST_DISTRICT_CODE }).Distinct();
                            //      query = from districtList in query
                            //              where !mappedDistricts_TA.Any(districtCode => districtCode.MAST_DISTRICT_CODE == districtList.MAST_DISTRICT_CODE)
                            //              select districtList;
                            /*Changed by Sammed Patil on 06June2014 
                             * Mapped Districts should not be visible in list for district Mapping but if end date is provided the make it available for mapping*/
                            var mappedDistricts_TA = (from agencyDistrictDetails in dbContext.ADMIN_TA_STATE where agencyDistrictDetails.ADMIN_TA_CODE == agencyCode && agencyDistrictDetails.MAST_END_DATE == null select new { agencyDistrictDetails.MAST_DISTRICT_CODE }).Distinct();
                            query = from districtList in query
                                    where !mappedDistricts_TA.Any(districtCode => districtCode.MAST_DISTRICT_CODE == districtList.MAST_DISTRICT_CODE)
                                    select districtList;
                            break;
                        case MappingType.SRRDADistrict:
                            var mappedDistricts_SRRDA = (from SRRDADistrictDetails in dbContext.ADMIN_AGENCY_DISTRICT where SRRDADistrictDetails.ADMIN_ND_CODE == adminNdCode select new { SRRDADistrictDetails.MAST_DISTRICT_CODE }).Distinct();  //change by deepak on 01/10/2014
                            query = from districtList in query
                                    where !mappedDistricts_SRRDA.Any(districtCode => districtCode.MAST_DISTRICT_CODE == districtList.MAST_DISTRICT_CODE)
                                    select districtList;
                            break;

                    }


                }

                totalRecords = query.Count();

                if (rows != 0)
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {

                            switch (sidx)
                            {
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "MastDistrictId":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateName":
                                    query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsIAPDistrict":
                                    query = query.OrderBy(x => x.MAST_IAP_DISTRICT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "lockStatus":
                                    query = query.OrderBy(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                    break;
                            }



                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "MastDistrictId":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "StateName":
                                    query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsIAPDistrict":
                                    query = query.OrderByDescending(x => x.MAST_IAP_DISTRICT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "lockStatus":
                                    query = query.OrderByDescending(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                    }
                }
                else
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME);
                                    break;
                                case "MastDistrictId":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_ID);
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED);
                                    break;
                                case "IsIAPDistrict":
                                    query = query.OrderBy(x => x.MAST_IAP_DISTRICT);
                                    break;
                                case "lockStatus":
                                    query = query.OrderBy(x => x.MAST_LOCK_STATUS);
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME);
                                    break;
                            }
                        }
                        else
                        {

                            switch (sidx)
                            {
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME);
                                    break;
                                case "MastDistrictId":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_ID);
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED);
                                    break;
                                case "IsIAPDistrict":
                                    query = query.OrderByDescending(x => x.MAST_IAP_DISTRICT);
                                    break;
                                case "lockStatus":
                                    query = query.OrderByDescending(x => x.MAST_LOCK_STATUS);
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME);
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME);
                    }
                }

                var result = query.Select(districtDetails => new
                {
                    districtDetails.MAST_DISTRICT_CODE,
                    districtDetails.MAST_DISTRICT_NAME,
                    districtDetails.MAST_STATE_NAME,
                    districtDetails.MAST_PMGSY_INCLUDED,
                    districtDetails.MAST_IAP_DISTRICT,
                    districtDetails.MAST_STATE_CODE,
                    districtDetails.MAST_LOCK_STATUS,
                    districtDetails.MAST_DISTRICT_ID,
                    districtDetails.MAST_DISTRICT_ACTIVE
                }).ToArray();


                return result.Select(districtDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString(), "StateCode =" + districtDetails.MAST_STATE_CODE.ToString() }),

                    cell = new[] {   
                                districtDetails.MAST_DISTRICT_ID==null?"0": districtDetails.MAST_DISTRICT_ID.ToString().Trim(),                      
                                districtDetails.MAST_DISTRICT_NAME.ToString().Trim(),
                                districtDetails.MAST_STATE_NAME.ToString().Trim(),
                                districtDetails.MAST_PMGSY_INCLUDED.ToString().Trim()=="Y"?"Yes":"No",
                                districtDetails.MAST_IAP_DISTRICT.ToString().Trim()=="Y"?"Yes":"No",
                                districtDetails.MAST_DISTRICT_ACTIVE == "Y" ? "Yes" : "No",
                                URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + districtDetails.MAST_STATE_CODE.ToString(), "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}),
                             // PMGSYSession.Current.RoleCode == 23 ? URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) :((districtDetails.MAST_LOCK_STATUS=="N" || districtDetails.MAST_LOCK_STATUS=="M")?URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}):string.Empty),
                             // "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit District Details' onClick ='EditDistrictDetails(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete District Details' onClick ='DeleteDistrictDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
                                 PMGSYSession.Current.RoleCode == 23 ?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit District Details' onClick ='EditDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete District Details' onClick ='DeleteDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :districtDetails.MAST_LOCK_STATUS=="M"?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit District Details' onClick ='EditDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete District Details' onClick ='DeleteDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :(districtDetails.MAST_LOCK_STATUS=="N")  ? ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit District Details' onClick ='EditDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete District Details' onClick ='DeleteDistrictDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :("<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td> <td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"),
                                districtDetails.MAST_LOCK_STATUS=="N"?"No":districtDetails.MAST_LOCK_STATUS=="M"?"Unlock":"Yes",
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


        public MASTER_DISTRICT GetDistrictDetailsDAL_ByDistrictCode(int districtCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Models.MASTER_DISTRICT districtDetails = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();

                MASTER_DISTRICT master_district = null;

                if (districtDetails != null)
                {

                    master_district = new MASTER_DISTRICT()
                    {
                        //MAST_DISTRICT_CODE = districtDetails.MAST_DISTRICT_CODE,
                        EncryptedDistrictCode = URLEncrypt.EncryptParameters1(new string[] { "DistrictCode =" + districtDetails.MAST_DISTRICT_CODE.ToString() }),
                        MAST_DISTRICT_NAME = districtDetails.MAST_DISTRICT_NAME,
                        MAST_STATE_CODE = districtDetails.MAST_STATE_CODE,
                        IsPMGSYIncluded = districtDetails.MAST_PMGSY_INCLUDED == "Y" ? true : false,
                        IsIAPDistrict = districtDetails.MAST_IAP_DISTRICT == "Y" ? true : false,
                        Max_Mast_District_Id = districtDetails.MAST_DISTRICT_ID == null ? 0 : districtDetails.MAST_DISTRICT_ID,
                        IsActive = (districtDetails.MAST_DISTRICT_ACTIVE == "Y" ? true : false)
                    };
                }

                return master_district;

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


        public bool UpdateDistrictDetailsDAL(MASTER_DISTRICT master_district, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    int districtCode = 0;
                    encryptedParameters = master_district.EncryptedDistrictCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    districtCode = Convert.ToInt32(decryptedParameters["DistrictCode"].ToString());

                    Int32 recordCount = 0;
                    recordCount = dbContext.MASTER_DISTRICT.Where(district => district.MAST_DISTRICT_NAME.ToUpper() == master_district.MAST_DISTRICT_NAME.ToUpper() && district.MAST_STATE_CODE == master_district.MAST_STATE_CODE && district.MAST_DISTRICT_CODE != districtCode).Count();
                    if (recordCount > 0)
                    {
                        message = "District Name under selected state is already exist.";
                        return false;
                    }
                    recordCount = dbContext.MASTER_DISTRICT.Where(district => district.MAST_DISTRICT_ID == master_district.Max_Mast_District_Id && district.MAST_STATE_CODE == master_district.MAST_STATE_CODE && district.MAST_DISTRICT_CODE != districtCode).Count();
                    if (recordCount > 0)
                    {
                        message = "District Id under selected state is already exist.";
                        return false;
                    }

                    Models.MASTER_DISTRICT districtDetails = dbContext.MASTER_DISTRICT.Where(district => district.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();

                    districtDetails.MAST_DISTRICT_ID = master_district.Max_Mast_District_Id;
                    districtDetails.MAST_DISTRICT_NAME = master_district.MAST_DISTRICT_NAME;
                    // districtDetails.MAST_STATE_CODE = master_district.MAST_STATE_CODE;
                    districtDetails.MAST_PMGSY_INCLUDED = master_district.IsPMGSYIncluded == true ? "Y" : "N";
                    districtDetails.MAST_IAP_DISTRICT = master_district.IsIAPDistrict == true ? "Y" : "N";
                    districtDetails.MAST_DISTRICT_ACTIVE = master_district.IsActive == true ? "Y" : "N";
                    //new change done by Vikram as suggested by Srinivas sir on 08 Dec 2014



                    /* Int32? NICStateCode = (from state in dbContext.MASTER_STATE where state.MAST_STATE_CODE == master_district.MAST_STATE_CODE select (Int32)state.MAST_NIC_STATE_CODE).FirstOrDefault();
                     districtDetails.MAST_NIC_STATE_CODE = NICStateCode == null ? 0 : (Int32)NICStateCode;  */



                    //added by abhishek kamble 26-nov-2013                
                    districtDetails.USERID = PMGSYSession.Current.UserId;
                    districtDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(districtDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //if (dbContext.USP_UPDATE_MASTER_ACTIVE_STATUS(districtCode, "D", districtDetails.MAST_DISTRICT_ACTIVE).Select(m => m.Value).FirstOrDefault() > 0)
                    {
                        ts.Complete();
                        return true;
                    }
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

        #endregion DistrictMasterDataEntry

        #region BlockMasterDataEntry
        public Array GetBlockDetailsListDAL(bool isMap, bool isMLA, int stateCode, int districtCode, int MPMLAConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                //var query = from blockDetails in dbContext.MASTER_BLOCK
                //            join districtDetails in dbContext.MASTER_DISTRICT
                //            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                //            join stateDetails in dbContext.MASTER_STATE 
                //            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE 
                //            where blockDetails.MAST_BLOCK_ACTIVE == "Y"
                //            orderby stateDetails.MAST_STATE_NAME,districtDetails.MAST_DISTRICT_NAME,blockDetails.MAST_BLOCK_NAME
                //            select new { blockDetails.MAST_BLOCK_CODE, blockDetails.MAST_BLOCK_NAME, stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_IS_DESERT,blockDetails.MAST_IS_TRIBAL, blockDetails.MAST_PMGSY_INCLUDED };

                var query = from blockDetails in dbContext.MASTER_BLOCK
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            join stateDetails in dbContext.MASTER_STATE
                            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where
                                /*blockDetails.MAST_BLOCK_ACTIVE == "Y" &&
                                    districtDetails.MAST_DISTRICT_ACTIVE == "Y" && //Added By Abhishek kamble 5-May-2014
                                    stateDetails.MAST_STATE_ACTIVE == "Y"// Added By Abhishek kamble 5-May-2014 &&*/
                             (districtCode == 0 ? 1 : blockDetails.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode)
                            && (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                            orderby stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME
                            select new
                            {
                                blockDetails.MAST_BLOCK_CODE,
                                blockDetails.MAST_BLOCK_NAME,
                                blockDetails.MAST_IAP_BLOCK,
                                stateDetails.MAST_STATE_NAME,
                                districtDetails.MAST_DISTRICT_NAME,
                                districtDetails.MAST_IAP_DISTRICT,
                                blockDetails.MAST_IS_DESERT,
                                blockDetails.MAST_IS_TRIBAL,
                                blockDetails.MAST_PMGSY_INCLUDED,
                                blockDetails.MAST_SCHEDULE5,
                                blockDetails.MAST_DISTRICT_CODE,
                                blockDetails.MAST_BLOCK_ID,
                                blockDetails.MAST_BLOCK_ACTIVE,
                                MAST_LOCK_STATUS = (dbContext.UDF_IMS_UNLOCK_STATUS(blockDetails.MASTER_DISTRICT.MAST_STATE_CODE, blockDetails.MAST_DISTRICT_CODE, blockDetails.MAST_BLOCK_CODE, 0, 0, 0, 0, 0, "BM", 1, (short)PMGSYSession.Current.RoleCode).Select(m => m.UNLOCK_COUNT).FirstOrDefault() > 0 ? "M" : blockDetails.MAST_LOCK_STATUS),
                                blockDetails.MAST_IS_BADB,
                            };

                ///Commented by SAMMED A. PATIL on 25 APRIL 2017 to allow multiple mapping of Blocks for MLA constituency
                if (isMap)
                {
                    switch (isMLA)
                    {
                        case true:
                            var mappedblocks_MLA = (from blockDetails in dbContext.MASTER_MLA_BLOCKS where blockDetails.MAST_MLA_BLOCK_ACTIVE == "Y" && blockDetails.MAST_MLA_CONST_CODE == MPMLAConstituencyCode select new { blockDetails.MAST_BLOCK_CODE }).Distinct();
                            query = from blockList in query
                                    where !mappedblocks_MLA.Any(blockCode => blockCode.MAST_BLOCK_CODE == blockList.MAST_BLOCK_CODE)
                                    select blockList;
                            break;

                        case false:
                            var mappedblocks_MP = (from blockDetails in dbContext.MASTER_MP_BLOCKS where blockDetails.MAST_MP_BLOCK_ACTIVE == "Y" && blockDetails.MAST_MP_CONST_CODE == MPMLAConstituencyCode select new { blockDetails.MAST_BLOCK_CODE }).Distinct();
                            query = from blockList in query
                                    where !mappedblocks_MP.Any(blockCode => blockCode.MAST_BLOCK_CODE == blockList.MAST_BLOCK_CODE)
                                    select blockList;
                            break;
                    }

                }

                totalRecords = query == null ? 0 : query.Count();

                if (rows != 0)
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {

                            switch (sidx)
                            {
                                case "BlockName":
                                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "MastBlockId":
                                    query = query.OrderBy(x => x.MAST_BLOCK_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsDESERT":
                                    query = query.OrderBy(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsTRIBAL":
                                    query = query.OrderBy(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "lockStatus":
                                    query = query.OrderBy(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                    break;
                            }

                        }
                        else
                        {

                            switch (sidx)
                            {
                                case "BlockName":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                    break;
                                case "MastBlockId":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsDESERT":
                                    query = query.OrderByDescending(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsTRIBAL":
                                    query = query.OrderByDescending(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                case "lockStatus":
                                    query = query.OrderByDescending(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                    }
                }
                else
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {

                            switch (sidx)
                            {
                                case "BlockName":
                                    query = query.OrderBy(x => x.MAST_BLOCK_NAME);
                                    break;
                                case "MastBlockId":
                                    query = query.OrderBy(x => x.MAST_BLOCK_ID);
                                    break;
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME);
                                    break;
                                case "IsDESERT":
                                    query = query.OrderBy(x => x.MAST_IS_DESERT);
                                    break;
                                case "IsTRIBAL":
                                    query = query.OrderBy(x => x.MAST_IS_TRIBAL);
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED);
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderBy(x => x.MAST_SCHEDULE5);
                                    break;
                                case "lockStatus":
                                    query = query.OrderBy(x => x.MAST_LOCK_STATUS);
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME);
                                    break;
                            }


                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "BlockName":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME);
                                    break;
                                case "MastBlockId":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_ID);
                                    break;
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME);
                                    break;
                                case "IsDESERT":
                                    query = query.OrderByDescending(x => x.MAST_IS_DESERT);
                                    break;
                                case "IsTRIBAL":
                                    query = query.OrderByDescending(x => x.MAST_IS_TRIBAL);
                                    break;
                                case "IsPMGSYIncluded":
                                    query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED);
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderByDescending(x => x.MAST_SCHEDULE5);
                                    break;
                                case "lockStatus":
                                    query = query.OrderByDescending(x => x.MAST_LOCK_STATUS);
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME);
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME);
                    }

                }

                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(blockDetails => new
                 {
                     blockDetails.MAST_BLOCK_CODE,
                     blockDetails.MAST_BLOCK_NAME,
                     blockDetails.MAST_DISTRICT_NAME,
                     blockDetails.MAST_STATE_NAME,
                     blockDetails.MAST_IAP_DISTRICT,
                     blockDetails.MAST_IS_DESERT,
                     blockDetails.MAST_IS_TRIBAL,
                     blockDetails.MAST_PMGSY_INCLUDED,
                     blockDetails.MAST_SCHEDULE5,
                     blockDetails.MAST_DISTRICT_CODE,
                     blockDetails.MAST_LOCK_STATUS,
                     blockDetails.MAST_IAP_BLOCK,
                     blockDetails.MAST_BLOCK_ID,
                     blockDetails.MAST_BLOCK_ACTIVE,
                     blockDetails.MAST_IS_BADB
                 }).ToArray();


                return result.Select(blockDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString() }),

                    cell = new[] {                         

                                blockDetails.MAST_BLOCK_ID==null?"0": blockDetails.MAST_BLOCK_ID.ToString().Trim() ,                              
                                blockDetails.MAST_BLOCK_NAME.ToString().Trim() ,                              
                                blockDetails.MAST_DISTRICT_NAME.ToString().Trim() , 
                                blockDetails.MAST_STATE_NAME.ToString().Trim(),
                                blockDetails.MAST_IAP_DISTRICT.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_IAP_BLOCK.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_IS_DESERT.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_IS_TRIBAL.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_PMGSY_INCLUDED.ToString().Trim()=="Y"?"Yes":"No",  
                                blockDetails.MAST_SCHEDULE5==null?"No":blockDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No",  
                                blockDetails.MAST_BLOCK_ACTIVE == "Y" ? "Yes" : "No",
                                URLEncrypt.EncryptParameters1(new string[] {  "DistrictCode =" + blockDetails.MAST_DISTRICT_CODE.ToString(), "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}),
                              //  PMGSYSession.Current.RoleCode == 23 ? URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) :((blockDetails.MAST_LOCK_STATUS=="N" || blockDetails.MAST_LOCK_STATUS=="M")?URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}):string.Empty)
                                  
                              blockDetails.MAST_LOCK_STATUS=="N"?"No":blockDetails.MAST_LOCK_STATUS=="M"?"Unlock":"Yes",
                              blockDetails.MAST_IS_BADB.ToString().Trim()=="Y"?"Yes":"No",

                              PMGSYSession.Current.RoleCode == 23 ?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Block Details' onClick ='EditBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Block Details' onClick ='DeleteBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :blockDetails.MAST_LOCK_STATUS=="M"?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit Block Details' onClick ='EditBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete Block Details' onClick ='DeleteBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :(blockDetails.MAST_LOCK_STATUS=="N")  ? ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Block Details' onClick ='EditBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Block Details' onClick ='DeleteBlockDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :("<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td> <td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"),
                                 
                             }
                }).ToArray();

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MasterDataEntryDAL.GetBlockDetailsListDAL()");
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

        public List<Models.MASTER_DISTRICT> GetAllDistrictsByStateCode(int stateCode, bool isSearch)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                List<Models.MASTER_DISTRICT> districtList = null;

                if (PMGSYSession.Current.RoleCode == 23)
                {
                    districtList = dbContext.MASTER_DISTRICT.Where(d => d.MAST_STATE_CODE == stateCode).OrderBy(d => d.MAST_DISTRICT_NAME).ToList<Models.MASTER_DISTRICT>();
                }
                else
                {
                    districtList = dbContext.MASTER_DISTRICT.Where(d => d.MAST_STATE_CODE == stateCode && d.MAST_DISTRICT_ACTIVE == "Y").OrderBy(d => d.MAST_DISTRICT_NAME).ToList<Models.MASTER_DISTRICT>();
                }

                if (isSearch)
                {
                    districtList.Insert(0, new Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                }
                else
                {
                    districtList.Insert(0, new Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                }

                return districtList;

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


        public bool SaveBlockDetailsDAL(BlockMaster master_block, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Int32 recordCount = dbContext.MASTER_BLOCK.Where(block => block.MAST_DISTRICT_CODE == master_block.MAST_DISTRICT_CODE && block.MAST_BLOCK_NAME.ToUpper() == master_block.MAST_BLOCK_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "Block Name under selected district is already exist.";
                    return false;
                }
                Models.MASTER_BLOCK blockDetails = new Models.MASTER_BLOCK();


                blockDetails.MAST_BLOCK_CODE = (Int32)GetMaxCode(MasterDataEntryModules.BlockMaster);

                //Added By Abhishek kamble 18-Mar-2014 

                int? blockId = dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == master_block.MAST_DISTRICT_CODE).Max(s => (int?)s.MAST_BLOCK_ID);

                if (blockId == null)
                {
                    blockId = 1;
                }
                else
                {
                    blockId = blockId + 1;
                }

                blockDetails.MAST_BLOCK_ID = blockId;

                blockDetails.MAST_BLOCK_NAME = master_block.MAST_BLOCK_NAME;
                blockDetails.MAST_DISTRICT_CODE = master_block.MAST_DISTRICT_CODE;
                blockDetails.MAST_IS_DESERT = master_block.IsDESERT == true ? "Y" : "N";
                blockDetails.MAST_IS_BADB = master_block.IsBADB == true ? "Y" : "N";
                blockDetails.MAST_IS_TRIBAL = master_block.IsTRIBAL == true ? "Y" : "N";
                blockDetails.MAST_PMGSY_INCLUDED = master_block.IsPMGSYIncluded == true ? "Y" : "N";
                blockDetails.MAST_SCHEDULE5 = master_block.IsSchedule5 == true ? "Y" : "N";
                blockDetails.MAST_IAP_BLOCK = master_block.IsIAP == true ? "Y" : "N";
                blockDetails.MAST_BLOCK_ACTIVE = "Y";
                blockDetails.MAST_LOCK_STATUS = "N";

                Int32? NICStateCode = (from district in dbContext.MASTER_DISTRICT where district.MAST_DISTRICT_CODE == master_block.MAST_DISTRICT_CODE select (Int32)district.MAST_NIC_STATE_CODE).FirstOrDefault();
                blockDetails.MAST_NIC_STATE_CODE = NICStateCode == null ? 0 : (Int32)NICStateCode;

                //added by abhishek kamble 
                blockDetails.USERID = PMGSYSession.Current.UserId;
                blockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_BLOCK.Add(blockDetails);
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

        public BlockMaster GetBlockDetailsDAL_ByBlockCode(int blockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_BLOCK blockDetails = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).FirstOrDefault();

                BlockMaster master_block = null;

                if (blockDetails != null)
                {
                    Int32? stateCode = (from district in dbContext.MASTER_DISTRICT where district.MAST_DISTRICT_CODE == blockDetails.MAST_DISTRICT_CODE select (Int32)district.MAST_STATE_CODE).FirstOrDefault();

                    master_block = new BlockMaster()
                    {
                        // MAST_BLOCK_CODE = blockDetails.MAST_BLOCK_CODE,
                        EncryptedBlockCode = URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockDetails.MAST_BLOCK_CODE.ToString() }),
                        MAST_STATE_CODE = stateCode == null ? 0 : (Int32)stateCode,
                        MAST_DISTRICT_CODE = blockDetails.MAST_DISTRICT_CODE,
                        MAST_BLOCK_NAME = blockDetails.MAST_BLOCK_NAME,
                        IsDESERT = blockDetails.MAST_IS_DESERT == "Y" ? true : false,
                        IsBADB = blockDetails.MAST_IS_BADB == "Y" ? true : false,
                        IsTRIBAL = blockDetails.MAST_IS_TRIBAL == "Y" ? true : false,
                        IsPMGSYIncluded = blockDetails.MAST_PMGSY_INCLUDED == "Y" ? true : false,
                        IsSchedule5 = blockDetails.MAST_SCHEDULE5 == null ? false : blockDetails.MAST_SCHEDULE5 == "Y" ? true : false,
                        IsIAP = blockDetails.MAST_IAP_BLOCK == null ? false : blockDetails.MAST_IAP_BLOCK == "Y" ? true : false,
                        Max_Mast_Block_Id = blockDetails.MAST_BLOCK_ID == null ? 0 : blockDetails.MAST_BLOCK_ID,
                        IsActive = (blockDetails.MAST_BLOCK_ACTIVE == "Y" ? true : false)
                    };
                }

                return master_block;

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


        public bool UpdateBlockDetailsDAL(BlockMaster master_block, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            using (TransactionScope ts = new TransactionScope())
            {

                try
                {

                    int blockCode = 0;
                    encryptedParameters = master_block.EncryptedBlockCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                    Int32 recordCount = 0;
                    recordCount = dbContext.MASTER_BLOCK.Where(block => block.MAST_BLOCK_NAME.ToUpper() == master_block.MAST_BLOCK_NAME.ToUpper() && block.MAST_DISTRICT_CODE == master_block.MAST_DISTRICT_CODE && block.MAST_BLOCK_CODE != blockCode).Count();
                    if (recordCount > 0)
                    {
                        message = "Block Name under selected district is already exist.";
                        return false;
                    }
                    recordCount = dbContext.MASTER_BLOCK.Where(block => block.MAST_BLOCK_ID == master_block.Max_Mast_Block_Id && block.MAST_DISTRICT_CODE == master_block.MAST_DISTRICT_CODE && block.MAST_BLOCK_CODE != blockCode).Count();
                    if (recordCount > 0)
                    {
                        message = "Block Id under selected district is already exist.";
                        return false;
                    }

                    Models.MASTER_BLOCK blockDetails = dbContext.MASTER_BLOCK.Where(block => block.MAST_BLOCK_CODE == blockCode).FirstOrDefault();

                    blockDetails.MAST_BLOCK_ID = master_block.Max_Mast_Block_Id;
                    blockDetails.MAST_BLOCK_NAME = master_block.MAST_BLOCK_NAME;
                    //blockDetails.MAST_DISTRICT_CODE = master_block.MAST_DISTRICT_CODE;
                    blockDetails.MAST_IS_DESERT = master_block.IsDESERT == true ? "Y" : "N";
                    blockDetails.MAST_IS_BADB = master_block.IsBADB == true ? "Y" : "N";
                    blockDetails.MAST_IS_TRIBAL = master_block.IsTRIBAL == true ? "Y" : "N";
                    blockDetails.MAST_PMGSY_INCLUDED = master_block.IsPMGSYIncluded == true ? "Y" : "N";
                    blockDetails.MAST_SCHEDULE5 = master_block.IsSchedule5 == true ? "Y" : "N";
                    blockDetails.MAST_IAP_BLOCK = master_block.IsIAP == true ? "Y" : "N";
                    //new change done by Vikram as suggested by Srinivas sir on 12 Dec 2014
                    blockDetails.MAST_BLOCK_ACTIVE = master_block.IsActive == true ? "Y" : "N";


                    /* Int32? NICStateCode = (from state in dbContext.MASTER_STATE where state.MAST_STATE_CODE == master_district.MAST_STATE_CODE select (Int32)state.MAST_NIC_STATE_CODE).FirstOrDefault();
                     districtDetails.MAST_NIC_STATE_CODE = NICStateCode == null ? 0 : (Int32)NICStateCode;*/

                    //added by abhishek kamble  26-nov-2013
                    blockDetails.USERID = PMGSYSession.Current.UserId;
                    blockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(blockDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 25-Feb-2014 for IsSchedule 5 changes for Habitation start

                    //All Villages Isschedule5 flag of selected block changed to IsSchedule5 Y/N and Active flag set according to the block active flag
                    //dbContext.MASTER_VILLAGE.Where(m => m.MAST_BLOCK_CODE == blockCode).ToList().ForEach(m => { m.MAST_SCHEDULE5 = blockDetails.MAST_SCHEDULE5; m.MAST_VILLAGE_ACTIVE = blockDetails.MAST_BLOCK_ACTIVE; });
                    //dbContext.SaveChanges();




                    if (dbContext.USP_UPDATE_VILLAGE_HABITATION_STATUS(blockDetails.MAST_BLOCK_CODE, blockDetails.MAST_BLOCK_ACTIVE, blockDetails.MAST_SCHEDULE5).Select(m => m.Value).FirstOrDefault() > 0)
                    {
                        //if (dbContext.USP_UPDATE_MASTER_ACTIVE_STATUS(blockDetails.MAST_BLOCK_CODE, "B", blockDetails.MAST_BLOCK_ACTIVE).Select(m => m.Value).FirstOrDefault() > 0)
                        {
                            ts.Complete();
                        }
                    }

                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (UpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
            }//end of transaction scope
        }

        #endregion BlockMasterDataEntry

        #region VillageMasterDataEntry
        public Array GetVillageDetailsListDAL(int stateCode, int districtCode, int blockCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int PMGSYYear = PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011;


                //Int16 PMGSYScheme=PMGSYSession.Current.PMGSYScheme;
                //  if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                /*var query = from villageDetails in dbContext.MASTER_VILLAGE
                            join blockDetails in dbContext.MASTER_BLOCK
                            on villageDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            join stateDetails in dbContext.MASTER_STATE
                            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where villageDetails.MAST_VILLAGE_ACTIVE == "Y"
                            orderby stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME,villageDetails.MAST_VILLAGE_NAME
                            select new {villageDetails.MAST_VILLAGE_CODE,villageDetails.MAST_VILLAGE_NAME,  stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME,blockDetails.MAST_BLOCK_NAME, villageDetails.MAST_VILLAGE_TOT_POP,villageDetails.MAST_VILLAGE_SCST_POP };*/

                //for search
                //var query = from villageDetails in dbContext.MASTER_VILLAGE

                //            join blockDetails in dbContext.MASTER_BLOCK
                //            on villageDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                //            join districtDetails in dbContext.MASTER_DISTRICT
                //            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                //            join stateDetails in dbContext.MASTER_STATE
                //            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                //             join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION 
                //            on villageDetails.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE into villData
                //                from vd in villData.DefaultIfEmpty()
                //            where villageDetails.MAST_VILLAGE_ACTIVE == "Y" &&
                //            (blockCode == 0 ? 1 : villageDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                //            (districtCode == 0 ? 1 : blockDetails.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                //            (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                //            vd.MAST_CENSUS_YEAR == (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2 )
                //            orderby stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, villageDetails.MAST_VILLAGE_NAME
                //            select new { villageDetails.MAST_VILLAGE_CODE, villageDetails.MAST_VILLAGE_NAME, stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, vd.MAST_VILLAGE_TOT_POP, vd.MAST_VILLAGE_SCST_POP, villageDetails.MAST_SCHEDULE5, villageDetails.MAST_BLOCK_CODE, villageDetails.MAST_LOCK_STATUS };

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                List<PMGSY.Models.USP_GET_VILLAGE_DETAILS_LIST_Result> query = new List<PMGSY.Models.USP_GET_VILLAGE_DETAILS_LIST_Result>();
                query = dbContext.USP_GET_VILLAGE_DETAILS_LIST(stateCode, districtCode, blockCode, PMGSYYear, roleCode).ToList<PMGSY.Models.USP_GET_VILLAGE_DETAILS_LIST_Result>();


                totalRecords = query == null ? 0 : query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "VillageName":
                                query = query.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "BlockName":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "DistrictName":
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                query = query.OrderBy(x => x.MAST_VILLAGE_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SCSTPopulation":
                                query = query.OrderBy(x => x.MAST_VILLAGE_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IsSchedule5":
                                query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "lockStatus":
                                query = query.OrderBy(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "VillageName":
                                query = query.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "BlockName":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "DistrictName":
                                query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                query = query.OrderByDescending(x => x.MAST_VILLAGE_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SCSTPopulation":
                                query = query.OrderByDescending(x => x.MAST_VILLAGE_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IsSchedule5":
                                query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "lockStatus":
                                query = query.OrderByDescending(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                // query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(villageDetails => new
                {
                    villageDetails.MAST_VILLAGE_CODE,
                    villageDetails.MAST_VILLAGE_NAME,
                    villageDetails.MAST_STATE_NAME,
                    villageDetails.MAST_DISTRICT_NAME,
                    villageDetails.MAST_IAP_DISTRICT,
                    villageDetails.MAST_BLOCK_NAME,
                    villageDetails.MAST_VILLAGE_TOT_POP,
                    villageDetails.MAST_VILLAGE_SCST_POP,
                    villageDetails.MAST_SCHEDULE5,
                    villageDetails.MAST_BLOCK_CODE,
                    villageDetails.MAST_LOCK_STATUS,
                    villageDetails.MAST_VILLAGE_ACTIVE
                }).ToArray();


                return result.Select(villageDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString() }),
                    cell = new[] {                         

                                villageDetails.MAST_VILLAGE_NAME.Trim() == string.Empty? "NA": villageDetails.MAST_VILLAGE_NAME.Trim(),                    
                                villageDetails.MAST_BLOCK_NAME.ToString().Trim() , 
                                villageDetails.MAST_DISTRICT_NAME.ToString().Trim() , 
                                villageDetails.MAST_STATE_NAME.ToString().Trim(),
                                villageDetails.MAST_IAP_DISTRICT.ToString().Trim()=="Y"?"Yes":"No",
                                villageDetails.MAST_VILLAGE_TOT_POP.ToString(),   
                                villageDetails.MAST_VILLAGE_SCST_POP.ToString(),  
                                villageDetails.MAST_SCHEDULE5.ToString().Trim(),
                                villageDetails.MAST_VILLAGE_ACTIVE.ToString().Trim(),
                                //villageDetails.MAST_SCHEDULE5==null?"No":villageDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No",  
                               // villageDetails.MAST_ENTRY_DATE == null ? "NA": Convert.ToDateTime(villageDetails.MAST_ENTRY_DATE.ToString()).ToString("dd/MM/yyyy"),                               
                                URLEncrypt.EncryptParameters1(new string[] {  "BlockCode =" + villageDetails.MAST_BLOCK_CODE.ToString(), "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString() }),
                               
                                // 
                               //villageDetails.MAST_VILLAGE_ACTIVE.ToString().Trim()=="Yes"?(dbContext.VILLAGE_SHIFTING_TRACTING.Any(m=>m.MAST_NEW_VILLAGE_ID==villageDetails.MAST_VILLAGE_CODE)?  "<a href='#'  class='ui-icon ui-icon-unlocked ui-align-center' onClick='ShiftVillageBlockNew(\""+ URLEncrypt.EncryptParameters1(new string[] {  "BlockCode =" + villageDetails.MAST_BLOCK_CODE.ToString(), "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString() }) +"\"); return false;'>Shift Village Block </a>":"-"):"NA",
                               villageDetails.MAST_VILLAGE_ACTIVE.ToString().Trim()=="Yes"?(dbContext.VILLAGE_SHIFTING_TRACTING.Any(m=>m.MAST_NEW_VILLAGE_ID==villageDetails.MAST_VILLAGE_CODE)||(stateCode==14 || stateCode==15 )?  "<a href='#'  class='ui-icon ui-icon-unlocked ui-align-center' onClick='ShiftVillageBlockNew(\""+ URLEncrypt.EncryptParameters1(new string[] {  "BlockCode =" + villageDetails.MAST_BLOCK_CODE.ToString(), "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString() }) +"\"); return false;'>Shift Village Block </a>":"-"):"NA",
                                
                                
                                
                                
                                //PMGSYSession.Current.RoleCode == 23 ?URLEncrypt.EncryptParameters1(new string[] { "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}):villageDetails.MAST_LOCK_STATUS=="N"?URLEncrypt.EncryptParameters1(new string[] { "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}):string.Empty
                                 PMGSYSession.Current.RoleCode == 23 ?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Village Details' onClick ='EditVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Village Details' onClick ='DeleteVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :villageDetails.MAST_LOCK_STATUS=="M"?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit Village Details' onClick ='EditVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete Village Details' onClick ='DeleteVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :(villageDetails.MAST_LOCK_STATUS=="N")  ? ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Village Details' onClick ='EditVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Village Details' onClick ='DeleteVillageDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :("<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td> <td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"),
                                 villageDetails.MAST_LOCK_STATUS=="N"?"No":villageDetails.MAST_LOCK_STATUS=="M"?"Unlock":"Yes",
             
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

        public List<Models.MASTER_BLOCK> GetAllBlocksByDistrictCode(int districtCode, bool isSearch)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                List<Models.MASTER_BLOCK> blockList = null;

                if (PMGSYSession.Current.RoleCode == 23)
                {
                    blockList = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode).OrderBy(b => b.MAST_BLOCK_NAME).ToList<Models.MASTER_BLOCK>();
                }
                else
                {
                    blockList = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode && b.MAST_BLOCK_ACTIVE == "Y").OrderBy(b => b.MAST_BLOCK_NAME).ToList<Models.MASTER_BLOCK>();
                }

                if (isSearch)
                {
                    blockList.Insert(0, new Models.MASTER_BLOCK() { MAST_BLOCK_CODE = 0, MAST_BLOCK_NAME = "All Blocks" });
                }
                else
                {
                    blockList.Insert(0, new Models.MASTER_BLOCK() { MAST_BLOCK_CODE = 0, MAST_BLOCK_NAME = "--Select--" });
                }

                return blockList;

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


        public bool SaveVillageDetailsDAL(VillageMaster master_village, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int PMGSYYear1 = 2001;
                int PMGSYYear2 = 2011;

                Int16 PMGSYScheme = PMGSYSession.Current.PMGSYScheme;

                Int32 recordCount = dbContext.MASTER_VILLAGE.Where(village => village.MAST_BLOCK_CODE == master_village.MAST_BLOCK_CODE && village.MAST_VILLAGE_NAME.ToUpper() == master_village.MAST_VILLAGE_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "Village Name under selected block is already exist.";
                    return false;
                }
                Models.MASTER_VILLAGE villageDetails = new Models.MASTER_VILLAGE();
                Models.MASTER_VILLAGE_POPULATION villagePopulationDetails = new Models.MASTER_VILLAGE_POPULATION();

                using (var scope = new TransactionScope())
                {
                    villageDetails.MAST_VILLAGE_CODE = (Int32)GetMaxCode(MasterDataEntryModules.VillageMaster);
                    villagePopulationDetails.MAST_VILLAGE_CODE = villageDetails.MAST_VILLAGE_CODE;//(Int32)GetMaxCode(MasterDataEntryModules.VillageMaster);

                    villageDetails.MAST_VILLAGE_NAME = master_village.MAST_VILLAGE_NAME;
                    villageDetails.MAST_BLOCK_CODE = master_village.MAST_BLOCK_CODE;
                    villageDetails.MAST_VILLAGE_TOT_POP = (Int32)master_village.MAST_VILLAGE_TOT_POP;       //to be commented later
                    villageDetails.MAST_VILLAGE_SCST_POP = (Int32)master_village.MAST_VILLAGE_SCST_POP;     //to be commented later
                    villagePopulationDetails.MAST_VILLAGE_TOT_POP = (Int32)master_village.MAST_VILLAGE_TOT_POP;
                    villagePopulationDetails.MAST_VILLAGE_SCST_POP = (Int32)master_village.MAST_VILLAGE_SCST_POP;
                    villagePopulationDetails.MAST_CENSUS_YEAR = (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2);
                    villagePopulationDetails.MAST_LOCK_STATUS = "N";
                    villageDetails.MAST_SCHEDULE5 = master_village.IsSchedule5 == true ? "Y" : "N";
                    //villageDetails.MAST_ENTRY_DATE = DateTime.Now;
                    villageDetails.MAST_VILLAGE_ACTIVE = "Y";
                    villageDetails.MAST_LOCK_STATUS = "N";

                    Int32? NICStateCode = (from block in dbContext.MASTER_BLOCK where block.MAST_BLOCK_CODE == master_village.MAST_BLOCK_CODE select (Int32)block.MAST_NIC_STATE_CODE).FirstOrDefault();
                    villageDetails.MAST_NIC_STATE_CODE = NICStateCode == null ? 0 : (Int32)NICStateCode;

                    //added by abhishek kamble 26-nov-2013
                    villageDetails.USERID = PMGSYSession.Current.UserId;
                    villageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    villagePopulationDetails.USERID = PMGSYSession.Current.UserId;
                    villagePopulationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                    dbContext.MASTER_VILLAGE.Add(villageDetails);
                    //dbContext.SaveChanges();
                    dbContext.MASTER_VILLAGE_POPULATION.Add(villagePopulationDetails);
                    dbContext.SaveChanges();
                    scope.Complete();
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


        public VillageMaster GetVillageDetailsDAL_ByVillageCode(int villageCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int PMGSYYear1 = 2001;
                int PMGSYYear2 = 2011;

                Int16 PMGSYScheme = PMGSYSession.Current.PMGSYScheme;


                Models.MASTER_VILLAGE villageDetails = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).FirstOrDefault();


                VillageMaster master_village = null;

                if (villageDetails != null)
                {
                    Int32? districtCode = (from block in dbContext.MASTER_BLOCK where block.MAST_BLOCK_CODE == villageDetails.MAST_BLOCK_CODE select (Int32)block.MAST_DISTRICT_CODE).FirstOrDefault();


                    Int32? stateCode = districtCode == null ? 0 : (from district in dbContext.MASTER_DISTRICT where district.MAST_DISTRICT_CODE == districtCode select (Int32)district.MAST_STATE_CODE).FirstOrDefault();


                    Models.MASTER_VILLAGE_POPULATION villagePopulationDetails = dbContext.MASTER_VILLAGE_POPULATION.Where(v => v.MAST_VILLAGE_CODE == villageCode && v.MAST_CENSUS_YEAR == (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2)).FirstOrDefault();



                    master_village = new VillageMaster()
                    {
                        // MAST_VILLAGE_CODE=villageDetails.MAST_VILLAGE_CODE,
                        EncryptedVillageCode = URLEncrypt.EncryptParameters1(new string[] { "VillageCode =" + villageDetails.MAST_VILLAGE_CODE.ToString() }),
                        MAST_VILLAGE_NAME = villageDetails.MAST_VILLAGE_NAME,
                        MAST_STATE_CODE = stateCode == null ? 0 : (Int32)stateCode,
                        MAST_DISTRICT_CODE = districtCode == null ? 0 : (Int32)districtCode,
                        MAST_BLOCK_CODE = villageDetails.MAST_BLOCK_CODE,
                        //MAST_VILLAGE_TOT_POP = villageDetails.MAST_VILLAGE_TOT_POP,
                        // MAST_VILLAGE_SCST_POP = villageDetails.MAST_VILLAGE_SCST_POP,

                        MAST_VILLAGE_TOT_POP = villagePopulationDetails == null ? 0 : villagePopulationDetails.MAST_VILLAGE_TOT_POP,
                        MAST_VILLAGE_SCST_POP = villagePopulationDetails == null ? 0 : villagePopulationDetails.MAST_VILLAGE_SCST_POP,
                        IsSchedule5 = villageDetails.MAST_SCHEDULE5 == null ? false : villageDetails.MAST_SCHEDULE5 == "Y" ? true : false,
                        IsActive = villageDetails.MAST_VILLAGE_ACTIVE == "Y" ? true : false
                    };

                }

                return master_village;

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


        public bool UpdateVillageDetailsDAL(VillageMaster master_village, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            using (TransactionScope ts = new TransactionScope())
            {

                try
                {

                    int PMGSYYear1 = 2001;
                    int PMGSYYear2 = 2011;

                    Int16 PMGSYScheme = PMGSYSession.Current.PMGSYScheme;

                    int villageCode = 0;
                    encryptedParameters = master_village.EncryptedVillageCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());

                    //Int32 recordCount = dbContext.MASTER_VILLAGE.Where(village => village.MAST_VILLAGE_NAME.ToUpper() == master_village.MAST_VILLAGE_NAME.ToUpper() && village.MAST_BLOCK_CODE == master_village.MAST_BLOCK_CODE && village.MAST_VILLAGE_CODE != villageCode).Count();

                    //if (recordCount > 0)
                    //{
                    //    message = "Village Name under selected block is already exist.";
                    //    return false;
                    //}

                    //added by ujjwal saket on 21-10-2013 for PMGSY II
                    Int32 schemeCheck = dbContext.MASTER_VILLAGE_POPULATION.Where(villagePopulation => villagePopulation.MAST_VILLAGE_CODE == villageCode && villagePopulation.MAST_CENSUS_YEAR == (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2)).Count();


                    Models.MASTER_VILLAGE villageDetails = dbContext.MASTER_VILLAGE.Where(village => village.MAST_VILLAGE_CODE == villageCode).FirstOrDefault();

                    if (schemeCheck == 0)
                    {
                        // Models.MASTER_VILLAGE villageDetailsSave = new Models.MASTER_VILLAGE();
                        Models.MASTER_VILLAGE_POPULATION villagePopulationDetailsSave = new Models.MASTER_VILLAGE_POPULATION();

                        // using (var scope = new TransactionScope())
                        {

                            //  villageDetailsSave.MAST_VILLAGE_CODE = (Int32)GetMaxCode(MasterDataEntryModules.VillageMaster);
                            villagePopulationDetailsSave.MAST_VILLAGE_CODE = villageDetails.MAST_VILLAGE_CODE; //(Int32)GetMaxCode(MasterDataEntryModules.VillageMaster);


                            villagePopulationDetailsSave.MAST_VILLAGE_TOT_POP = (Int32)master_village.MAST_VILLAGE_TOT_POP;
                            villagePopulationDetailsSave.MAST_VILLAGE_SCST_POP = (Int32)master_village.MAST_VILLAGE_SCST_POP;
                            villagePopulationDetailsSave.MAST_CENSUS_YEAR = (PMGSYScheme == 1 ? (Int32)PMGSYYear1 : (Int32)PMGSYYear2);

                            villagePopulationDetailsSave.MAST_LOCK_STATUS = villageDetails.MAST_LOCK_STATUS;

                            dbContext.MASTER_VILLAGE_POPULATION.Add(villagePopulationDetailsSave);
                            dbContext.SaveChanges();
                            ts.Complete();
                        }

                        return true;
                    }



                    Models.MASTER_VILLAGE_POPULATION villagePopulationDetails = dbContext.MASTER_VILLAGE_POPULATION.Where(villagePopulation => (villagePopulation.MAST_VILLAGE_CODE == villageCode) && villagePopulation.MAST_CENSUS_YEAR == (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2)).FirstOrDefault();

                    //Added By Abhishek kamble 5-feb-2014 start 

                    //select SUM(mhd.MAST_HAB_TOT_POP),SUM(mhd.MAST_HAB_SCST_POP) from
                    //OMMAS_DEV.omms.MASTER_HABITATIONS mh
                    //join OMMAS_DEV.omms.MASTER_HABITATIONS_DETAILS mhd
                    //on mh.MAST_HAB_CODE=mhd.MAST_HAB_CODE
                    //where mh.MAST_VILLAGE_CODE=6616041
                    //and mhd.MAST_YEAR=2011

                    var villageHabitationTotalPopulationDetails = (from mh in dbContext.MASTER_HABITATIONS
                                                                   join mhd in dbContext.MASTER_HABITATIONS_DETAILS
                                                                   on mh.MAST_HAB_CODE equals mhd.MAST_HAB_CODE
                                                                   where mh.MAST_VILLAGE_CODE == villageCode
                                                                   &&
                                                                   mhd.MAST_YEAR == (PMGSYScheme == 1 ? PMGSYYear1 : PMGSYYear2)
                                                                   select new
                                                                   {
                                                                       mhd.MAST_HAB_TOT_POP,
                                                                       mhd.MAST_HAB_SCST_POP
                                                                   }
                                                                     );


                    if (villageHabitationTotalPopulationDetails != null)
                    {
                        Int64? habTotalPop = villageHabitationTotalPopulationDetails.Sum(s => (Int64?)s.MAST_HAB_TOT_POP);
                        Int64? habTotalSCSTPop = villageHabitationTotalPopulationDetails.Sum(s => (Int64?)s.MAST_HAB_SCST_POP);

                        //validation logic start

                        if (master_village.MAST_VILLAGE_TOT_POP < habTotalPop && master_village.MAST_VILLAGE_SCST_POP < habTotalSCSTPop)
                        {
                            message = "Village total population and village total SC/ST population must be greater than or equal to All habitation Total population '" + habTotalPop + "' and Total SC/ST Population '" + habTotalSCSTPop + "' ";
                            return false;
                        }
                        if (master_village.MAST_VILLAGE_TOT_POP < habTotalPop)
                        {
                            message = "Village total population must be greater than or equal to All habitation Total population '" + habTotalPop + "'";
                            return false;
                        }
                        if (master_village.MAST_VILLAGE_SCST_POP < habTotalSCSTPop)
                        {
                            message = "Village total SC/ST population must be greater than or equal to All habitation Total SC/ST Population '" + habTotalSCSTPop + "' ";
                            return false;
                        }
                    }
                    //Added By Abhishek kamble 5-feb-2014 end

                    villageDetails.MAST_VILLAGE_NAME = master_village.MAST_VILLAGE_NAME;
                    villageDetails.MAST_VILLAGE_TOT_POP = (Int32)master_village.MAST_VILLAGE_TOT_POP;       //to be commented later
                    villageDetails.MAST_VILLAGE_SCST_POP = (Int32)master_village.MAST_VILLAGE_SCST_POP;     //to be commented later

                    villagePopulationDetails.MAST_VILLAGE_TOT_POP = (Int32)master_village.MAST_VILLAGE_TOT_POP;
                    villagePopulationDetails.MAST_VILLAGE_SCST_POP = (Int32)master_village.MAST_VILLAGE_SCST_POP;
                    villageDetails.MAST_VILLAGE_ACTIVE = master_village.IsActive == true ? "Y" : "N";

                    villageDetails.MAST_SCHEDULE5 = master_village.IsSchedule5 == true ? "Y" : "N";

                    //added by abhishek kamble 26-nov-2013
                    villageDetails.USERID = PMGSYSession.Current.UserId;
                    villageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    villagePopulationDetails.USERID = PMGSYSession.Current.UserId;
                    villagePopulationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(villageDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.Entry(villagePopulationDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 25-Feb-2014 for IsSchedule 5 changes for Habitation start
                    //dbContext.MASTER_HABITATIONS.Where(m => m.MAST_VILLAGE_CODE == villageCode).ToList().ForEach(m => m.MAST_SCHEDULE5 = villageDetails.MAST_SCHEDULE5);
                    //dbContext.SaveChanges();

                    ///Commented on 25JAN2019 to restrict Habitation status updation on updating village master
                    //if (dbContext.USP_UPDATE_MASTER_ACTIVE_STATUS(villageDetails.MAST_VILLAGE_CODE, "V", villageDetails.MAST_VILLAGE_ACTIVE).Select(m => m.Value).FirstOrDefault() > 0)
                    {
                        ts.Complete();
                    }

                    //Added By Abhishek kamble 25-Feb-2014 for IsSchedule 5 changes for Habitation end

                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (UpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        #endregion VillageMasterDataEntry

        #region HabitationMasterDataEntry
        public List<Models.MASTER_CENSUS_YEAR> GetCensusYears(bool flag)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                List<Models.MASTER_CENSUS_YEAR> yearList = new List<Models.MASTER_CENSUS_YEAR>();
                //yearList = (from year in dbContext.MASTER_CENSUS_YEAR orderby year.MAST_YEAR select year).ToList<Models.MASTER_CENSUS_YEAR>();


                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    yearList = (from year in dbContext.MASTER_CENSUS_YEAR orderby year.MAST_YEAR where year.MAST_CENSUS_YEAR.Equals(2001) select year).ToList<Models.MASTER_CENSUS_YEAR>();
                }
                else
                {
                    yearList = (from year in dbContext.MASTER_CENSUS_YEAR orderby year.MAST_YEAR where year.MAST_CENSUS_YEAR.Equals(2011) select year).ToList<Models.MASTER_CENSUS_YEAR>();
                }


                //if (flag)
                //{
                //    yearList.Insert(0, new PMGSY.Models.MASTER_CENSUS_YEAR() { MAST_CENSUS_YEAR = 0, MAST_YEAR = "All Years" });
                //}

                return yearList;
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


        public Array GetHabitationDetailsListDAL(bool isMap, int stateCode, int districtCode, int blockCode, string villageName, string habitationName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                /*var query = from habitationDetails in dbContext.MASTER_HABITATIONS
                            join villageDetails in dbContext.MASTER_VILLAGE
                            on habitationDetails.MAST_VILLAGE_CODE equals villageDetails.MAST_VILLAGE_CODE
                            join mpContituency in dbContext.MASTER_MP_CONSTITUENCY
                            on habitationDetails.MAST_MP_CONST_CODE equals mpContituency.MAST_MP_CONST_CODE
                            join mlaContituency in dbContext.MASTER_MLA_CONSTITUENCY
                            on habitationDetails.MAST_MLA_CONST_CODE equals mlaContituency.MAST_MLA_CONST_CODE
                            join blockDetails in dbContext.MASTER_BLOCK
                            on villageDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            //join stateDetails in dbContext.MASTER_STATE
                            //on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where habitationDetails.MAST_HABITATION_ACTIVE == "Y"
                            orderby  districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, villageDetails.MAST_VILLAGE_NAME,habitationDetails.MAST_HAB_NAME
                            select new { habitationDetails.MAST_HAB_CODE, habitationDetails.MAST_HAB_NAME, villageDetails.MAST_VILLAGE_NAME,  districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, mpContituency.MAST_MP_CONST_NAME,mlaContituency.MAST_MLA_CONST_NAME, villageDetails.MAST_VILLAGE_TOT_POP};*/
                /* Int32? villageCode = null;

                 if (villageName != string.Empty)
                 {
                    villageCode = (from village in dbContext.MASTER_VILLAGE where village.MAST_BLOCK_CODE == blockCode && village.MAST_VILLAGE_NAME == villageName.Trim() select (Int32)village.MAST_VILLAGE_CODE).FirstOrDefault();
                 }*/

                villageName = villageName.Replace("*", "");
                //villageName = villageName.Replace("%", ""); //Commented By Abhishek kamble 11-Mar-2014
                villageName = villageName + "%";
                habitationName = habitationName.Replace("*", "");
                //habitationName = habitationName.Replace("%", ""); //Commented By Abhishek kamble 11-Mar-2014
                habitationName = habitationName + "%";

                //working query but due to calling stored procedure it has commeneted 

                /*  var query = from habitationDetails in dbContext.MASTER_HABITATIONS
                          join villageDetails in dbContext.MASTER_VILLAGE
                          on habitationDetails.MAST_VILLAGE_CODE equals villageDetails.MAST_VILLAGE_CODE
                          join mpContituency in dbContext.MASTER_MP_CONSTITUENCY
                          on habitationDetails.MAST_MP_CONST_CODE equals mpContituency.MAST_MP_CONST_CODE into mpConstituencies
                          from mpConstituency in mpConstituencies.DefaultIfEmpty()
                          join mlaContituency in dbContext.MASTER_MLA_CONSTITUENCY
                          on habitationDetails.MAST_MLA_CONST_CODE equals mlaContituency.MAST_MLA_CONST_CODE into mlaConstituencies
                          from mlaConstituency in mlaConstituencies.DefaultIfEmpty()
                          join blockDetails in dbContext.MASTER_BLOCK
                          on villageDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                          join districtDetails in dbContext.MASTER_DISTRICT
                          on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                          join stateDetails in dbContext.MASTER_STATE
                          on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                          where 
                               habitationDetails.MAST_HABITATION_ACTIVE == "Y" &&
                            //(villageCode == null ? 1 : habitationDetails.MAST_VILLAGE_CODE) == (villageCode == null ? 1 : villageCode) &&
                          (villageName == string.Empty ? "%" : villageDetails.MAST_VILLAGE_NAME.ToUpper()).StartsWith(villageName == string.Empty ? "%" : villageName.ToUpper()) &&
                          (blockCode == 0 ? 1 : villageDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                          (districtCode == 0 ? 1 : blockDetails.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                          (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                          //orderby districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, villageDetails.MAST_VILLAGE_NAME, habitationDetails.MAST_HAB_NAME
                          select new { habitationDetails.MAST_HAB_CODE, habitationDetails.MAST_HAB_NAME, villageDetails.MAST_VILLAGE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, mpConstituency.MAST_MP_CONST_NAME, mlaConstituency.MAST_MLA_CONST_NAME, habitationDetails.MAST_SCHEDULE5, habitationDetails.MAST_LOCK_STATUS };*/

                //Time Out Added By Abhishek kamble 11-Mar-2014 
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //List<Models.USP_MAS_List_Habitaion_Details_Result> query = dbContext.USP_MAS_List_Habitaion_Details(stateCode, districtCode, blockCode, villageName, habitationName, page, rows, sidx, sord).ToList<Models.USP_MAS_List_Habitaion_Details_Result>();

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                List<Models.USP_MAS_List_Habitaion_Details_Result> query = dbContext.USP_MAS_List_Habitaion_Details(stateCode, districtCode, blockCode, roleCode, villageName, habitationName, page, rows, sidx, sord, PMGSYSession.Current.PMGSYScheme).ToList<Models.USP_MAS_List_Habitaion_Details_Result>();


                //long? count = (from habitationDetails in dbContext.MASTER_HABITATIONS
                //               join villageDetails in dbContext.MASTER_VILLAGE
                //               on habitationDetails.MAST_VILLAGE_CODE equals villageDetails.MAST_VILLAGE_CODE
                //               join mpContituency in dbContext.MASTER_MP_CONSTITUENCY
                //               on habitationDetails.MAST_MP_CONST_CODE equals mpContituency.MAST_MP_CONST_CODE into mpConstituencies
                //               from mpConstituency in mpConstituencies.DefaultIfEmpty()
                //               join mlaContituency in dbContext.MASTER_MLA_CONSTITUENCY
                //               on habitationDetails.MAST_MLA_CONST_CODE equals mlaContituency.MAST_MLA_CONST_CODE into mlaConstituencies
                //               from mlaConstituency in mlaConstituencies.DefaultIfEmpty()
                //               join blockDetails in dbContext.MASTER_BLOCK
                //               on villageDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                //               join districtDetails in dbContext.MASTER_DISTRICT
                //               on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                //               join stateDetails in dbContext.MASTER_STATE
                //               on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                //               where
                //               habitationDetails.MAST_HABITATION_ACTIVE == "Y" &&
                //               (villageName == string.Empty ? "%" : villageDetails.MAST_VILLAGE_NAME.ToUpper()).StartsWith(villageName == string.Empty ? "%" : villageName.ToUpper()) &&
                //               (habitationName == string.Empty ? "%" : habitationDetails.MAST_HAB_NAME.ToUpper()).StartsWith(habitationName == string.Empty ? "%" : habitationName.ToUpper()) &&
                //               (blockCode == 0 ? 1 : villageDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                //               (districtCode == 0 ? 1 : blockDetails.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                //               (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)

                //               select habitationDetails.MAST_HAB_CODE).Count();

                ////Modified By Abhishek kamble 2-Apr-2014
                ////totalRecords = count == null ? 0 : (long)count;
                //List<Models.USP_MAS_List_Habitaion_Details_Result> queryData = dbContext.USP_MAS_List_Habitaion_Details(stateCode, districtCode, blockCode, villageName, habitationName, 0, 0, sidx, sord).ToList<Models.USP_MAS_List_Habitaion_Details_Result>();
                // totalRecords = (long)query.Select(m => m.TOTAL_RECORDS).FirstOrDefault(); //change by deepak 16 Sept 2014
                totalRecords = query == null ? 0 : query.Count();
                if (isMap)
                {
                    var mappedHabs = (from panchayatHabitationDetails in dbContext.MASTER_PANCHAYAT_HABITATIONS where panchayatHabitationDetails.MAST_PAN_HAB_ACTIVE == "Y" select new { panchayatHabitationDetails.MAST_HAB_CODE }).Distinct().ToList();

                    //List<Models.MASTER_PANCHAYAT_HABITATIONS> mappedHabs = (from panchayatHabitationDetails in dbContext.MASTER_PANCHAYAT_HABITATIONS where panchayatHabitationDetails.MAST_PAN_HAB_ACTIVE == "Y" select panchayatHabitationDetails).ToList<Models.MASTER_PANCHAYAT_HABITATIONS>();

                    query = (from habitaionList in query
                             where !mappedHabs.Any(habCode => habCode.MAST_HAB_CODE == habitaionList.MAST_HAB_CODE)
                             select habitaionList).ToList();

                    totalRecords = query == null ? 0 : query.Count();

                }


                //working but due to paging in stored procedure it has been commented
                totalRecords = query == null ? 0 : query.Count();

                if (rows != 0)
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "HabitationName":
                                    query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "VillageName":
                                    query = query.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "BlockName":
                                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;

                                case "MPContituency":
                                    query = query.OrderBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "MLAContituency":
                                    query = query.OrderBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                case "LockStatus":
                                    query = query.OrderBy(x => x.UNLOCK_BY_MORD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_VILLAGE_NAME).ThenBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }


                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "HabitationName":
                                    query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "VillageName":
                                    query = query.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "BlockName":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "MPContituency":
                                    query = query.OrderByDescending(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "MLAContituency":
                                    query = query.OrderByDescending(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                case "LockStatus":
                                    query = query.OrderByDescending(x => x.UNLOCK_BY_MORD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }

                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_VILLAGE_NAME).ThenBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "HabitationCode":
                                    query = query.OrderBy(x => x.MAST_HAB_CODE).ToList();
                                    break;
                                case "HabitationName":
                                    query = query.OrderBy(x => x.MAST_HAB_NAME).ToList();
                                    break;
                                case "VillageName":
                                    query = query.OrderBy(x => x.MAST_VILLAGE_NAME).ToList();
                                    break;
                                case "BlockName":
                                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).ToList();
                                    break;
                                case "DistrictName":
                                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ToList();
                                    break;
                                case "MPContituency":
                                    query = query.OrderBy(x => x.MAST_MP_CONST_NAME).ToList();
                                    break;
                                case "MLAContituency":
                                    query = query.OrderBy(x => x.MAST_MLA_CONST_NAME).ToList();
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderBy(x => x.MAST_SCHEDULE5).ToList();
                                    break;
                                case "LockStatus":
                                    query = query.OrderBy(x => x.UNLOCK_BY_MORD).ToList();
                                    break;
                                default:
                                    query = query.OrderBy(x => x.MAST_VILLAGE_NAME).ThenBy(x => x.MAST_HAB_NAME).ToList();
                                    break;
                            }



                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "HabitationCode":
                                    query = query.OrderByDescending(x => x.MAST_HAB_CODE).ToList();
                                    break;
                                case "HabitationName":
                                    query = query.OrderByDescending(x => x.MAST_HAB_NAME).ToList();
                                    break;
                                case "VillageName":
                                    query = query.OrderByDescending(x => x.MAST_VILLAGE_NAME).ToList();
                                    break;
                                case "BlockName":
                                    query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).ToList();
                                    break;
                                case "DistrictName":
                                    query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).ToList();
                                    break;
                                case "MPContituency":
                                    query = query.OrderByDescending(x => x.MAST_MP_CONST_NAME).ToList();
                                    break;
                                case "MLAContituency":
                                    query = query.OrderByDescending(x => x.MAST_MLA_CONST_NAME).ToList();
                                    break;
                                case "IsSchedule5":
                                    query = query.OrderByDescending(x => x.MAST_SCHEDULE5).ToList();
                                    break;
                                case "LockStatus":
                                    query = query.OrderByDescending(x => x.UNLOCK_BY_MORD).ToList();
                                    break;
                                default:
                                    query = query.OrderByDescending(x => x.MAST_HAB_NAME).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        query = query.OrderBy(x => x.MAST_VILLAGE_NAME).ThenBy(x => x.MAST_HAB_NAME).ToList();
                    }
                }
                //for testing

                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(habitationDetails => new
                {
                    habitationDetails.MAST_HAB_CODE,
                    habitationDetails.MAST_HAB_NAME,
                    habitationDetails.MAST_DISTRICT_NAME,
                    habitationDetails.MAST_IAP_DISTRICT,
                    habitationDetails.MAST_BLOCK_NAME,
                    habitationDetails.MAST_VILLAGE_NAME,
                    habitationDetails.MAST_STATE_NAME,
                    habitationDetails.MAST_MP_CONST_NAME,
                    habitationDetails.MAST_MLA_CONST_NAME,
                    habitationDetails.MAST_SCHEDULE5,
                    habitationDetails.MAST_LOCK_STATUS,
                    habitationDetails.UNLOCK_BY_MORD,
                    habitationDetails.MAST_HABITATION_ACTIVE

                }).ToArray();


                return result.Select(habitationDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString() }),
                    cell = new[] {                         
                                habitationDetails.MAST_HAB_CODE.ToString().Trim(),
                                habitationDetails.MAST_HAB_NAME.Trim() == string.Empty? "NA": habitationDetails.MAST_HAB_NAME.Trim(),     
                               // habitationDetails.MAST_HAB_NAME.Trim(),  
                                habitationDetails.MAST_VILLAGE_NAME.ToString().Trim(),
                                habitationDetails.MAST_BLOCK_NAME.ToString().Trim() ,  
                                habitationDetails.MAST_DISTRICT_NAME.ToString().Trim(),  
                                habitationDetails.MAST_STATE_NAME.ToString().Trim(),                              
                                habitationDetails.MAST_MP_CONST_NAME==null?"NA":habitationDetails.MAST_MP_CONST_NAME.ToString().Trim(),
                                habitationDetails.MAST_MLA_CONST_NAME==null?"NA":habitationDetails.MAST_MLA_CONST_NAME.ToString().Trim(),
                                //habitationDetails.MAST_VILLAGE_TOT_POP == null ? "NA": habitationDetails.MAST_VILLAGE_TOT_POP.ToString(),  
                                habitationDetails.MAST_IAP_DISTRICT.ToString().Trim()=="Y"?"Yes":"No",
                                habitationDetails.MAST_SCHEDULE5==null?"No":habitationDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No", 
                                habitationDetails.MAST_HABITATION_ACTIVE.ToString(),


                                // Checkpoint 1
                              habitationDetails.MAST_HABITATION_ACTIVE.ToString()=="Yes"?(dbContext.HABITATION_SHIFTING_TRACTING.Any(m=>m.MAST_NEW_HAB_CODE==habitationDetails.MAST_HAB_CODE)?  "<a href='#'  class='ui-icon ui-icon-unlocked ui-align-center' onClick='ShiftHabitationNew(\""+ URLEncrypt.EncryptParameters1(new string[] {  "HabCode =" + habitationDetails.MAST_HAB_CODE.ToString() })+"\"); return false;'>Shift Habitation</a>":"-"):"NA",
                                
   

                              //  habitationDetails.MAST_LOCK_STATUS=="N"?URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}):string.Empty
                               /*habitationDetails.UNLOCK_BY_MORD=="M"*/ 
                              //  PMGSYSession.Current.RoleCode == 23 ?
                              //  ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> </tr></table></center>") 
                              //  :(habitationDetails.UNLOCK_BY_MORD=="N")  ? ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> </tr></table></center>") 
                              //  :"<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                              //  ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Habitation Details' onClick ='ViewHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td></tr></table></center>")
                              
                              PMGSYSession.Current.PMGSYScheme == 1 
                                ?   "<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"

                              :  (PMGSYSession.Current.RoleCode == 23 ?
                              ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> </tr></table></center>") 
                                :(habitationDetails.UNLOCK_BY_MORD=="M")  ? 
                                ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> </tr></table></center>") 
                               :(habitationDetails.UNLOCK_BY_MORD=="N")  ?
                               ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Add Other Habitation Details' onClick ='AddOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/",""), "LockStatus="+habitationDetails.UNLOCK_BY_MORD }) + "\");'></span></td> <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Habitation Details' onClick ='EditHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Details' onClick ='DeleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td> </tr></table></center>") 
                               :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"),


                                ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Habitation Details' onClick ='ViewHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString().Replace("/","")}) + "\");'></span></td></tr></table></center>"),
                                 habitationDetails.UNLOCK_BY_MORD=="N"?"No":habitationDetails.UNLOCK_BY_MORD=="M"?"Unlock":"Yes",

 
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

        public List<Models.MASTER_VILLAGE> GetAllVillagesByBlockCode(int blockCode, bool isSearch)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                List<Models.MASTER_VILLAGE> villageList = null;

                if (PMGSYSession.Current.RoleCode == 23)
                {
                    villageList = dbContext.MASTER_VILLAGE.Where(v => v.MAST_BLOCK_CODE == blockCode).OrderBy(v => v.MAST_VILLAGE_NAME).ToList<Models.MASTER_VILLAGE>();
                }
                else
                {
                    villageList = dbContext.MASTER_VILLAGE.Where(v => v.MAST_BLOCK_CODE == blockCode && v.MAST_VILLAGE_ACTIVE == "Y").OrderBy(v => v.MAST_VILLAGE_NAME).ToList<Models.MASTER_VILLAGE>();
                }

                if (!isSearch)
                {
                    villageList.Insert(0, new Models.MASTER_VILLAGE() { MAST_VILLAGE_CODE = 0, MAST_VILLAGE_NAME = "--Select--" });
                }

                return villageList;

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

        public List<SelectListItem> GetAllHabitationNameByBlockCode(int blockCode, bool isSearch)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var villageList = dbContext.MASTER_VILLAGE.Where(v => v.MAST_BLOCK_CODE == blockCode && v.MAST_VILLAGE_ACTIVE == "Y").OrderBy(v => v.MAST_VILLAGE_NAME).Select(s => s.MAST_VILLAGE_CODE).ToList();

                var habList = (from hab in dbContext.MASTER_HABITATIONS
                               where villageList.Contains(hab.MAST_VILLAGE_CODE)
                               select new
                                      {
                                          hab.MAST_HAB_NAME,
                                          hab.MAST_HAB_CODE,
                                          hab.MASTER_VILLAGE.MAST_VILLAGE_TOT_POP

                                      }).ToList();
                List<SelectListItem> lstHabs = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                foreach (var data in habList)
                {
                    item = new SelectListItem();
                    item.Text = data.MAST_HAB_NAME + " " + "(HabCode: " + data.MAST_HAB_CODE.ToString() + " Pop: " + data.MAST_VILLAGE_TOT_POP.ToString() + " )";
                    item.Value = data.MAST_HAB_CODE.ToString();
                    lstHabs.Add(item);
                }
                //var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                //                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                //                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE

                //                      where item.MAST_BLOCK_CODE == villageCode //&&
                //                      //habitationDetails.MAST_HAB_CONNECTED == "Y"
                //                      select new
                //                      {
                //                          habitation.MAST_HAB_NAME,
                //                          habitation.MAST_HAB_CODE,

                //                      });



                return lstHabs;

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


        public List<Models.MASTER_MP_CONSTITUENCY> GetAllMPContituencyByBlockCode(int blockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var result = (from mpContituency in dbContext.MASTER_MP_CONSTITUENCY
                              join mpBlocks in dbContext.MASTER_MP_BLOCKS
                              on mpContituency.MAST_MP_CONST_CODE equals mpBlocks.MAST_MP_CONST_CODE
                              where mpBlocks.MAST_BLOCK_CODE == blockCode && mpContituency.MAST_MP_CONST_ACTIVE == "Y"
                              orderby mpContituency.MAST_MP_CONST_NAME
                              select new { mpContituency.MAST_MP_CONST_CODE, mpContituency.MAST_MP_CONST_NAME });

                List<Models.MASTER_MP_CONSTITUENCY> mpContituencyList = new List<Models.MASTER_MP_CONSTITUENCY>();

                foreach (var item in result)
                {
                    mpContituencyList.Add(new Models.MASTER_MP_CONSTITUENCY() { MAST_MP_CONST_CODE = item.MAST_MP_CONST_CODE, MAST_MP_CONST_NAME = item.MAST_MP_CONST_NAME });
                }

                //var result = mpContituencyList.Select(mpContituencyDetails => new
                //{
                //    mpContituencyDetails.MAST_MP_CONST_CODE,
                //    mpContituencyDetails.MAST_MP_CONST_NAME
                //}).ToList<Models.MASTER_MP_CONSTITUENCY>();                

                mpContituencyList.Insert(0, new Models.MASTER_MP_CONSTITUENCY() { MAST_MP_CONST_CODE = 0, MAST_MP_CONST_NAME = "--Select--" });

                return mpContituencyList;

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

        public List<Models.MASTER_MLA_CONSTITUENCY> GetAllMLAContituencyByBlockCode(int blockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var result = (from mlaContituency in dbContext.MASTER_MLA_CONSTITUENCY
                              join mlaBlocks in dbContext.MASTER_MLA_BLOCKS
                              on mlaContituency.MAST_MLA_CONST_CODE equals mlaBlocks.MAST_MLA_CONST_CODE
                              where mlaBlocks.MAST_BLOCK_CODE == blockCode && mlaContituency.MAST_MLA_CONST_ACTIVE == "Y"
                              orderby mlaContituency.MAST_MLA_CONST_NAME
                              select new { mlaContituency.MAST_MLA_CONST_CODE, mlaContituency.MAST_MLA_CONST_NAME });

                List<Models.MASTER_MLA_CONSTITUENCY> mlaContituencyList = new List<Models.MASTER_MLA_CONSTITUENCY>();

                foreach (var item in result)
                {
                    mlaContituencyList.Add(new Models.MASTER_MLA_CONSTITUENCY() { MAST_MLA_CONST_CODE = item.MAST_MLA_CONST_CODE, MAST_MLA_CONST_NAME = item.MAST_MLA_CONST_NAME });
                }


                mlaContituencyList.Insert(0, new Models.MASTER_MLA_CONSTITUENCY() { MAST_MLA_CONST_CODE = 0, MAST_MLA_CONST_NAME = "--Select--" });

                return mlaContituencyList;

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


        public bool SaveHabitationDetailsDAL(HabitationMaster master_habitations, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {



                Int32 recordCount = dbContext.MASTER_HABITATIONS.Where(habitation => habitation.MAST_VILLAGE_CODE == master_habitations.MAST_VILLAGE_CODE && habitation.MAST_HAB_NAME.ToUpper() == master_habitations.MAST_HAB_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "Habitation Name under selected village is already exist.";
                    return false;
                }
                Models.MASTER_HABITATIONS habitationDetails = new Models.MASTER_HABITATIONS();


                habitationDetails.MAST_HAB_CODE = (Int32)GetMaxCode(MasterDataEntryModules.HabitationMaster);


                habitationDetails.MAST_HAB_NAME = master_habitations.MAST_HAB_NAME;
                habitationDetails.MAST_VILLAGE_CODE = master_habitations.MAST_VILLAGE_CODE;

                if (master_habitations.MAST_MP_CONST_CODE != 0)
                {
                    habitationDetails.MAST_MP_CONST_CODE = master_habitations.MAST_MP_CONST_CODE;
                }
                if (master_habitations.MAST_MLA_CONST_CODE != 0)
                {
                    habitationDetails.MAST_MLA_CONST_CODE = master_habitations.MAST_MLA_CONST_CODE;
                }

                habitationDetails.MAST_SCHEDULE5 = master_habitations.IsSchedule5 == true ? "Y" : "N";

                /* habitationDetails.MAST_HAB_TOT_POP = master_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)master_habitations.MAST_HAB_TOT_POP;
                 habitationDetails.MAST_HAB_SCST_POP = master_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)master_habitations.MAST_HAB_SCST_POP;

                   habitationDetails.MAST_HAB_CONNECTED = master_habitations.HasHabConnected == true ? "Y" : "N";
                   habitationDetails.MAST_PANCHAYAT_HQ = master_habitations.ISPanchayatHQ == true ? "Y" : "N";

                   habitationDetails.MAST_PRIMARY_SCHOOL = master_habitations.HasPrimarySchool == true ? "Y" : "N";
                   habitationDetails.MAST_MIDDLE_SCHOOL = master_habitations.HasMiddleSchool == true ? "Y" : "N";
                   habitationDetails.MAST_HIGH_SCHOOL = master_habitations.HasHighSchool == true ? "Y" : "N";
                   habitationDetails.MAST_INTERMEDIATE_SCHOOL = master_habitations.HasIntermediateSchool == true ? "Y" : "N";
                   habitationDetails.MAST_DEGREE_COLLEGE = master_habitations.HasDegreeCollege == true ? "Y" : "N";
                   habitationDetails.MAST_HEALTH_SERVICE = master_habitations.HasHealthService == true ? "Y" : "N"; 
                   habitationDetails.MAST_DISPENSARY = master_habitations.HasDespensary == true ? "Y" : "N";
                   habitationDetails.MAST_PHCS = master_habitations.HasPHCS == true ? "Y" : "N";
                   habitationDetails.MAST_VETNARY_HOSPITAL = master_habitations.HasVetnaryHospital == true ? "Y" : "N";
                   habitationDetails.MAST_MCW_CENTERS = master_habitations.HasMCWCenters == true ? "Y" : "N";
                   habitationDetails.MAST_TELEGRAPH_OFFICE = master_habitations.HasTelegraphOffice == true ? "Y" : "N";
                   habitationDetails.MAST_TELEPHONE_CONNECTION = master_habitations.HasTelephoneConnection == true ? "Y" : "N";
                   habitationDetails.MAST_BUS_SERVICE = master_habitations.HasBusService == true ? "Y" : "N";
                   habitationDetails.MAST_RAILWAY_STATION = master_habitations.HasRailwayStation == true ? "Y" : "N";
                   habitationDetails.MAST_ELECTRICTY = master_habitations.HasElectricity == true ? "Y" : "N";
                   habitationDetails.MAST_TOURIST_PLACE = master_habitations.IsTouristPlace == true ? "Y" : "N";
                   habitationDetails.MAST_ENTRY_DATE = DateTime.Now;*/

                habitationDetails.MAST_HABITATION_ACTIVE = "Y";
                habitationDetails.MAST_LOCK_STATUS = "N";

                habitationDetails.MAST_HAB_STATUS = "U";

                //added by abhishek kamble 27-nov-2013
                habitationDetails.USERID = PMGSYSession.Current.UserId;
                habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_HABITATIONS.Add(habitationDetails);


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


        public HabitationMaster GetHabitationDetailsDAL_ByHabitationCode(int habitationCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_HABITATIONS habitationDetails = dbContext.MASTER_HABITATIONS.Where(h => h.MAST_HAB_CODE == habitationCode).FirstOrDefault();

                HabitationMaster master_habitation = null;

                if (habitationDetails != null)
                {
                    Int32? blockCode = (from village in dbContext.MASTER_VILLAGE where village.MAST_VILLAGE_CODE == habitationDetails.MAST_VILLAGE_CODE select (Int32)village.MAST_BLOCK_CODE).FirstOrDefault();

                    Int32? districtCode = blockCode == null ? 0 : (from block in dbContext.MASTER_BLOCK where block.MAST_BLOCK_CODE == blockCode select (Int32)block.MAST_DISTRICT_CODE).FirstOrDefault();

                    Int32? stateCode = districtCode == null ? 0 : (from district in dbContext.MASTER_DISTRICT where district.MAST_DISTRICT_CODE == districtCode select (Int32)district.MAST_STATE_CODE).FirstOrDefault();


                    master_habitation = new HabitationMaster()
                    {
                        // MAST_HAB_CODE = habitationDetails.MAST_HAB_CODE,
                        EncryptedHabitationCode = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString() }),
                        MAST_HAB_NAME = habitationDetails.MAST_HAB_NAME,
                        MAST_STATE_CODE = stateCode == null ? 0 : (Int32)stateCode,
                        MAST_DISTRICT_CODE = districtCode == null ? 0 : (Int32)districtCode,
                        MAST_BLOCK_CODE = blockCode == null ? 0 : (Int32)blockCode,
                        MAST_VILLAGE_CODE = habitationDetails.MAST_VILLAGE_CODE,
                        MAST_MP_CONST_CODE = habitationDetails.MAST_MP_CONST_CODE == null ? 0 : (Int32)habitationDetails.MAST_MP_CONST_CODE,
                        MAST_MLA_CONST_CODE = habitationDetails.MAST_MLA_CONST_CODE == null ? 0 : (Int32)habitationDetails.MAST_MLA_CONST_CODE,
                        IsSchedule5 = habitationDetails.MAST_SCHEDULE5 == null ? false : habitationDetails.MAST_SCHEDULE5 == "Y" ? true : false,
                        IsActive = habitationDetails.MAST_HABITATION_ACTIVE == "Y" ? true : false
                        /*MAST_HAB_TOT_POP = habitationDetails.MAST_HAB_TOT_POP,
                        MAST_HAB_SCST_POP = habitationDetails.MAST_HAB_SCST_POP, 
                        HasHabConnected = habitationDetails.MAST_HAB_CONNECTED == "Y" ? true : false,
                        HasPrimarySchool = habitationDetails.MAST_PRIMARY_SCHOOL == "Y" ? true : false,
                        HasMiddleSchool = habitationDetails.MAST_MIDDLE_SCHOOL == "Y" ? true : false,
                        HasHighSchool = habitationDetails.MAST_HIGH_SCHOOL == "Y" ? true : false,
                        HasIntermediateSchool = habitationDetails.MAST_INTERMEDIATE_SCHOOL == "Y" ? true : false,
                        HasDegreeCollege =habitationDetails.MAST_DEGREE_COLLEGE == "Y" ? true : false,
                        HasHealthService = habitationDetails.MAST_HEALTH_SERVICE == "Y" ? true : false,
                        HasDespensary = habitationDetails.MAST_DISPENSARY == "Y" ? true : false,
                        HasMCWCenters = habitationDetails.MAST_MCW_CENTERS == "Y" ? true : false,
                        HasPHCS = habitationDetails.MAST_PHCS == "Y" ? true : false,
                        HasVetnaryHospital = habitationDetails.MAST_VETNARY_HOSPITAL == "Y" ? true : false,
                        HasTelegraphOffice = habitationDetails.MAST_TELEGRAPH_OFFICE == "Y" ? true : false,
                        HasTelephoneConnection = habitationDetails.MAST_TELEPHONE_CONNECTION == "Y" ? true : false,
                        HasBusService = habitationDetails.MAST_BUS_SERVICE == "Y" ? true : false,
                        HasRailwayStation = habitationDetails.MAST_RAILWAY_STATION == "Y" ? true : false,
                        HasElectricity = habitationDetails.MAST_ELECTRICTY == "Y" ? true : false,
                        ISPanchayatHQ = habitationDetails.MAST_PANCHAYAT_HQ == "Y" ? true : false,
                        IsTouristPlace = habitationDetails.MAST_TOURIST_PLACE == "Y" ? true : false*/

                    };
                }

                return master_habitation;

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


        public bool UpdateHabitationDetailsDAL(HabitationMaster master_habitations, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                int habitationCode = 0;
                encryptedParameters = master_habitations.EncryptedHabitationCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());

                Int32 recordCount = dbContext.MASTER_HABITATIONS.Where(habitation => habitation.MAST_HAB_NAME.ToUpper() == master_habitations.MAST_HAB_NAME.ToUpper() && habitation.MAST_VILLAGE_CODE == master_habitations.MAST_VILLAGE_CODE && habitation.MAST_HAB_CODE != habitationCode).Count();

                if (recordCount > 0)
                {
                    message = "Habitation Name under selected block is already exist.";
                    return false;
                }

                Models.MASTER_HABITATIONS habitationDetails = dbContext.MASTER_HABITATIONS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode).FirstOrDefault();


                habitationDetails.MAST_HAB_NAME = master_habitations.MAST_HAB_NAME;

                if (master_habitations.MAST_MP_CONST_CODE != 0)
                {
                    habitationDetails.MAST_MP_CONST_CODE = master_habitations.MAST_MP_CONST_CODE;
                }
                else
                {
                    habitationDetails.MAST_MP_CONST_CODE = null;
                }


                if (master_habitations.MAST_MLA_CONST_CODE != 0)
                {
                    habitationDetails.MAST_MLA_CONST_CODE = master_habitations.MAST_MLA_CONST_CODE;
                }
                else
                {
                    habitationDetails.MAST_MLA_CONST_CODE = null;
                }

                habitationDetails.MAST_VILLAGE_CODE = master_habitations.MAST_VILLAGE_CODE;
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    habitationDetails.MAST_SCHEDULE5 = master_habitations.IsSchedule5 == true ? "Y" : "N";
                    habitationDetails.MAST_HABITATION_ACTIVE = master_habitations.IsActive == true ? "Y" : "N";
                }
                /* habitationDetails.MAST_HAB_TOT_POP = master_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)master_habitations.MAST_HAB_TOT_POP;
                 habitationDetails.MAST_HAB_SCST_POP = master_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)master_habitations.MAST_HAB_SCST_POP;
                 habitationDetails.MAST_HAB_CONNECTED = master_habitations.HasHabConnected == true ? "Y" : "N";
                 habitationDetails.MAST_PANCHAYAT_HQ = master_habitations.ISPanchayatHQ == true ? "Y" : "N";
                 habitationDetails.MAST_PRIMARY_SCHOOL = master_habitations.HasPrimarySchool == true ? "Y" : "N";
                 habitationDetails.MAST_MIDDLE_SCHOOL = master_habitations.HasMiddleSchool == true ? "Y" : "N";
                 habitationDetails.MAST_HIGH_SCHOOL = master_habitations.HasHighSchool == true ? "Y" : "N";
                 habitationDetails.MAST_INTERMEDIATE_SCHOOL = master_habitations.HasIntermediateSchool == true ? "Y" : "N";
                 habitationDetails.MAST_DEGREE_COLLEGE = master_habitations.HasDegreeCollege == true ? "Y" : "N";
                 habitationDetails.MAST_HEALTH_SERVICE = master_habitations.HasHealthService == true ? "Y" : "N";
                 habitationDetails.MAST_DISPENSARY = master_habitations.HasDespensary == true ? "Y" : "N";
                 habitationDetails.MAST_PHCS = master_habitations.HasPHCS == true ? "Y" : "N";
                 habitationDetails.MAST_VETNARY_HOSPITAL = master_habitations.HasVetnaryHospital == true ? "Y" : "N";
                 habitationDetails.MAST_MCW_CENTERS = master_habitations.HasMCWCenters == true ? "Y" : "N";
                 habitationDetails.MAST_TELEGRAPH_OFFICE = master_habitations.HasTelegraphOffice == true ? "Y" : "N";
                 habitationDetails.MAST_TELEPHONE_CONNECTION = master_habitations.HasTelephoneConnection == true ? "Y" : "N";
                 habitationDetails.MAST_BUS_SERVICE = master_habitations.HasBusService == true ? "Y" : "N";
                 habitationDetails.MAST_RAILWAY_STATION = master_habitations.HasRailwayStation == true ? "Y" : "N";
                 habitationDetails.MAST_ELECTRICTY = master_habitations.HasElectricity == true ? "Y" : "N";
                 habitationDetails.MAST_TOURIST_PLACE = master_habitations.IsTouristPlace == true ? "Y" : "N";*/


                //added by abhishek kamble 27-nov-2013
                habitationDetails.USERID = PMGSYSession.Current.UserId;
                habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(habitationDetails).State = System.Data.Entity.EntityState.Modified;
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

        #endregion HabitationMasterDataEntry

        #region PanchayatMasterDataEntry

        public Array GetPanchayatDetailsListDAL(int stateCode, int districtCode, int blockCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                /*var query = from panchayatDetails in dbContext.MASTER_PANCHAYAT
                            join blockDetails in dbContext.MASTER_BLOCK
                            on panchayatDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            join stateDetails in dbContext.MASTER_STATE
                            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where panchayatDetails.MAST_PANCHAYAT_ACTIVE == "Y"
                            orderby stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, panchayatDetails.MAST_PANCHAYAT_NAME
                            select new { panchayatDetails.MAST_PANCHAYAT_CODE, panchayatDetails.MAST_PANCHAYAT_NAME, stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME };*/

                var query = from panchayatDetails in dbContext.MASTER_PANCHAYAT
                            join blockDetails in dbContext.MASTER_BLOCK
                            on panchayatDetails.MASTER_BLOCK.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MASTER_DISTRICT.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            join stateDetails in dbContext.MASTER_STATE
                            on districtDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where panchayatDetails.MAST_PANCHAYAT_ACTIVE == "Y" &&
                            blockDetails.MAST_BLOCK_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            districtDetails.MAST_DISTRICT_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            stateDetails.MAST_STATE_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            (blockCode == 0 ? 1 : panchayatDetails.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                            (districtCode == 0 ? 1 : blockDetails.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                            (stateCode == 0 ? 1 : districtDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                            // orderby stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_BLOCK_NAME, panchayatDetails.MAST_PANCHAYAT_NAME
                            select new { panchayatDetails.MAST_PANCHAYAT_CODE, panchayatDetails.MAST_PANCHAYAT_NAME, stateDetails.MAST_STATE_NAME, districtDetails.MAST_DISTRICT_NAME, districtDetails.MAST_DISTRICT_CODE, blockDetails.MAST_BLOCK_NAME, panchayatDetails.MAST_BLOCK_CODE, panchayatDetails.MAST_LOCK_STATUS };


                totalRecords = query == null ? 0 : query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PanchyatName":
                                query = query.OrderBy(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "BlockName":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "LockStatus":
                                query = query.OrderBy(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }


                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "PanchyatName":
                                query = query.OrderByDescending(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "BlockName":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "LockStatus":
                                query = query.OrderByDescending(x => x.MAST_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_DISTRICT_NAME).ThenBy(x => x.MAST_BLOCK_NAME).ThenBy(x => x.MAST_PANCHAYAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(panchayatDetails => new
                {
                    panchayatDetails.MAST_PANCHAYAT_CODE,
                    panchayatDetails.MAST_PANCHAYAT_NAME,
                    panchayatDetails.MAST_STATE_NAME,
                    panchayatDetails.MAST_DISTRICT_NAME,
                    panchayatDetails.MAST_BLOCK_NAME,
                    panchayatDetails.MAST_BLOCK_CODE,
                    panchayatDetails.MAST_LOCK_STATUS,
                    panchayatDetails.MAST_DISTRICT_CODE


                }).ToArray();


                return result.Select(panchayatDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString() }),
                    cell = new[] {                         
                                panchayatDetails.MAST_PANCHAYAT_NAME.Trim() == string.Empty? "NA": panchayatDetails.MAST_PANCHAYAT_NAME.Trim(),
                                //panchayatDetails.MAST_PANCHAYAT_NAME.Trim(),                                                    
                                panchayatDetails.MAST_BLOCK_NAME.ToString().Trim() ,   
                                panchayatDetails.MAST_DISTRICT_NAME.ToString().Trim() ,
                                 panchayatDetails.MAST_STATE_NAME.ToString().Trim(), 
                                URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString(), "BlockCode =" + panchayatDetails.MAST_BLOCK_CODE.ToString(),"VillageCode ="+panchayatDetails.MAST_DISTRICT_CODE.ToString(), "PanchayatName="+  panchayatDetails.MAST_PANCHAYAT_NAME.ToString(),"StateName="+panchayatDetails.MAST_STATE_NAME.ToString(),"DistrictName="+panchayatDetails.MAST_DISTRICT_NAME,"BlockName="+panchayatDetails.MAST_BLOCK_NAME}),
                                URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString(), "BlockCode =" + panchayatDetails.MAST_BLOCK_CODE.ToString(),"VillageCode ="+panchayatDetails.MAST_DISTRICT_CODE.ToString(), "PanchayatName="+  panchayatDetails.MAST_PANCHAYAT_NAME.ToString(),"StateName="+panchayatDetails.MAST_STATE_NAME.ToString(),"DistrictName="+panchayatDetails.MAST_DISTRICT_NAME,"BlockName="+panchayatDetails.MAST_BLOCK_NAME}),
                                URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString(), "BlockCode =" + panchayatDetails.MAST_BLOCK_CODE.ToString()}),
                                //PMGSYSession.Current.RoleCode == 23 ?
                                //URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()})
                                //:panchayatDetails.MAST_LOCK_STATUS=="N"?
                                //URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()})
                                //:string.Empty,
                                   PMGSYSession.Current.RoleCode == 23 ?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Panchayat Details' onClick ='DeletePanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :panchayatDetails.MAST_LOCK_STATUS=="M"?
                                 ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit Panchayat Details' onClick ='EditPanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete Panchayat Details' onClick ='DeletePanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :(panchayatDetails.MAST_LOCK_STATUS=="N")  ? ("<center><table><tr>  <td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Village Details' onClick ='EditPanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] {  "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Village Details' onClick ='DeletePanchayatDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString()}) + "\");'></span></td></tr></table></center>")
                                 :("<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td> <td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>"),              
                                  panchayatDetails.MAST_LOCK_STATUS=="N"?"No":panchayatDetails.MAST_LOCK_STATUS=="M"?"Unlock":"Yes",
              
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


        public bool SavePanchayatDetailsDAL(PanchayatMaster master_panchayat, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Int32 recordCount = dbContext.MASTER_PANCHAYAT.Where(panchayat => panchayat.MAST_BLOCK_CODE == master_panchayat.MAST_BLOCK_CODE && panchayat.MAST_PANCHAYAT_NAME.ToUpper() == master_panchayat.MAST_PANCHAYAT_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "Panchayat Name under selected block is already exist.";
                    return false;
                }
                Models.MASTER_PANCHAYAT panchayatDetails = new Models.MASTER_PANCHAYAT();


                panchayatDetails.MAST_PANCHAYAT_CODE = (Int32)GetMaxCode(MasterDataEntryModules.PanchayatMaster);

                panchayatDetails.MAST_PANCHAYAT_NAME = master_panchayat.MAST_PANCHAYAT_NAME;
                panchayatDetails.MAST_BLOCK_CODE = master_panchayat.MAST_BLOCK_CODE;
                panchayatDetails.MAST_PANCHAYAT_ACTIVE = "Y";
                panchayatDetails.MAST_LOCK_STATUS = "N";


                //added by abhishek kamble 27-nov-2013
                panchayatDetails.USERID = PMGSYSession.Current.UserId;
                panchayatDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.MASTER_PANCHAYAT.Add(panchayatDetails);
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


        public PanchayatMaster GetPanchayatDetailsDAL_ByPanchayatCode(int panchayatCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_PANCHAYAT panchayatDetails = dbContext.MASTER_PANCHAYAT.Where(p => p.MAST_PANCHAYAT_CODE == panchayatCode).FirstOrDefault();

                PanchayatMaster master_panchayat = null;

                if (panchayatDetails != null)
                {
                    Int32? districtCode = (from block in dbContext.MASTER_BLOCK where block.MAST_BLOCK_CODE == panchayatDetails.MAST_BLOCK_CODE select (Int32)block.MAST_DISTRICT_CODE).FirstOrDefault();


                    Int32? stateCode = districtCode == null ? 0 : (from district in dbContext.MASTER_DISTRICT where district.MAST_DISTRICT_CODE == districtCode select (Int32)district.MAST_STATE_CODE).FirstOrDefault();


                    master_panchayat = new PanchayatMaster()
                    {
                        //MAST_PANCHAYAT_CODE = panchayatDetails.MAST_PANCHAYAT_CODE,
                        EncryptedPanchayatCode = URLEncrypt.EncryptParameters1(new string[] { "PanchayatCode =" + panchayatDetails.MAST_PANCHAYAT_CODE.ToString() }),
                        MAST_PANCHAYAT_NAME = panchayatDetails.MAST_PANCHAYAT_NAME,
                        MAST_STATE_CODE = stateCode == null ? 0 : (Int32)stateCode,
                        MAST_DISTRICT_CODE = districtCode == null ? 0 : (Int32)districtCode,
                        MAST_BLOCK_CODE = panchayatDetails.MAST_BLOCK_CODE
                    };
                }

                return master_panchayat;

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


        public bool UpdatePanchayatDetailsDAL(PanchayatMaster master_panchayat, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int panchayatCode = 0;
                encryptedParameters = master_panchayat.EncryptedPanchayatCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                panchayatCode = Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString());

                Int32 recordCount = dbContext.MASTER_PANCHAYAT.Where(panchayat => panchayat.MAST_PANCHAYAT_NAME.ToUpper() == master_panchayat.MAST_PANCHAYAT_NAME.ToUpper() && panchayat.MAST_BLOCK_CODE == master_panchayat.MAST_BLOCK_CODE && panchayat.MAST_PANCHAYAT_CODE != panchayatCode).Count();

                if (recordCount > 0)
                {
                    message = "Panchayat Name under selected block is already exist.";
                    return false;
                }

                Models.MASTER_PANCHAYAT panchayatDetails = dbContext.MASTER_PANCHAYAT.Where(panchayat => panchayat.MAST_PANCHAYAT_CODE == panchayatCode).FirstOrDefault();


                panchayatDetails.MAST_PANCHAYAT_NAME = master_panchayat.MAST_PANCHAYAT_NAME;

                //added by abhishek kamble 27-nov-2013
                panchayatDetails.USERID = PMGSYSession.Current.UserId;
                panchayatDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(panchayatDetails).State = System.Data.Entity.EntityState.Modified;
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

        #endregion PanchayatMasterDataEntry


        #region MLAConstituencyMasterDataEntry
        public Array GetMLAConstituencyDetailsListDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {

                var query = from mlaconstituencyDetails in dbContext.MASTER_MLA_CONSTITUENCY
                            join stateDetails in dbContext.MASTER_STATE
                            on mlaconstituencyDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where mlaconstituencyDetails.MAST_MLA_CONST_ACTIVE == "Y" &&
                            stateDetails.MAST_STATE_ACTIVE == "Y" &&//Added By Abhishek kamble
                            (stateCode == 0 ? 1 : mlaconstituencyDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                            select new { mlaconstituencyDetails.MAST_MLA_CONST_CODE, mlaconstituencyDetails.MAST_MLA_CONST_NAME, stateDetails.MAST_STATE_NAME, mlaconstituencyDetails.MAST_STATE_CODE, mlaconstituencyDetails.MAST_LOCK_STATUS };


                totalRecords = query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "MLAConstituencyName":
                                query = query.OrderBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateName":
                                query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }


                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MLAConstituencyName":
                                query = query.OrderByDescending(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateName":
                                query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }


                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(mlaconstituencydetails => new
                {
                    mlaconstituencydetails.MAST_MLA_CONST_CODE,
                    mlaconstituencydetails.MAST_MLA_CONST_NAME,
                    mlaconstituencydetails.MAST_STATE_NAME,
                    mlaconstituencydetails.MAST_STATE_CODE,
                    mlaconstituencydetails.MAST_LOCK_STATUS

                }).ToArray();


                return result.Select(mlaconstituencydetails => new
                {
                    cell = new[] {                         
                                mlaconstituencydetails.MAST_MLA_CONST_NAME.ToString().Trim()=="<Null>"? "NA":mlaconstituencydetails.MAST_MLA_CONST_NAME.ToString().Trim() , 
                                mlaconstituencydetails.MAST_STATE_NAME.ToString().Trim(),     
                                URLEncrypt.EncryptParameters1(new string[] { "MLAConstituencyCode =" + mlaconstituencydetails.MAST_MLA_CONST_CODE.ToString(), "StateCode =" + mlaconstituencydetails.MAST_STATE_CODE.ToString(), "MLAConstituencyName =" + mlaconstituencydetails.MAST_MLA_CONST_NAME.ToString(),"StateName ="+mlaconstituencydetails.MAST_STATE_NAME.ToString() }),
                                URLEncrypt.EncryptParameters1(new string[] { "MLAConstituencyCode =" + mlaconstituencydetails.MAST_MLA_CONST_CODE.ToString(), "StateCode =" + mlaconstituencydetails.MAST_STATE_CODE.ToString(), "MLAConstituencyName =" + mlaconstituencydetails.MAST_MLA_CONST_NAME.ToString(),"StateName ="+mlaconstituencydetails.MAST_STATE_NAME.ToString() }),
                                (PMGSYSession.Current.RoleCode == 23 || PMGSYSession.Current.RoleCode == 36) ?URLEncrypt.EncryptParameters1(new string[] { "MLAConstituencyCode =" + mlaconstituencydetails.MAST_MLA_CONST_CODE.ToString()}):mlaconstituencydetails.MAST_LOCK_STATUS=="N"?URLEncrypt.EncryptParameters1(new string[] { "MLAConstituencyCode =" + mlaconstituencydetails.MAST_MLA_CONST_CODE.ToString()}):string.Empty
                                 
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

        public bool SaveMLAConstituencyDetailsDAL(MLAConstituency master_mlaconstituency, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Int32 recordCount = dbContext.MASTER_MLA_CONSTITUENCY.Where(mlaconstituency => mlaconstituency.MAST_STATE_CODE == master_mlaconstituency.MAST_STATE_CODE && mlaconstituency.MAST_MLA_CONST_NAME.ToUpper() == master_mlaconstituency.MAST_MLA_CONST_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "MLA Constituency Name under selected state is already exist.";
                    return false;
                }
                Models.MASTER_MLA_CONSTITUENCY mlaconstituencyDetails = new Models.MASTER_MLA_CONSTITUENCY();


                mlaconstituencyDetails.MAST_MLA_CONST_CODE = (Int32)GetMaxCode(MasterDataEntryModules.MLAConstituency);
                mlaconstituencyDetails.MAST_MLA_CONST_NAME = master_mlaconstituency.MAST_MLA_CONST_NAME;
                mlaconstituencyDetails.MAST_STATE_CODE = master_mlaconstituency.MAST_STATE_CODE;

                mlaconstituencyDetails.MAST_MLA_CONST_ACTIVE = "Y";
                mlaconstituencyDetails.MAST_LOCK_STATUS = "N";

                //Added by abhishek kamble 27-nov-2013
                mlaconstituencyDetails.USERID = PMGSYSession.Current.UserId;
                mlaconstituencyDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_MLA_CONSTITUENCY.Add(mlaconstituencyDetails);
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

        public MLAConstituency GetMLAConstituencyDetailsDAL_ByMLAConstituencyCode(int mlaConstituencyCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_MLA_CONSTITUENCY mlaconstituencyDetails = dbContext.MASTER_MLA_CONSTITUENCY.Where(mla => mla.MAST_MLA_CONST_CODE == mlaConstituencyCode).FirstOrDefault();

                MLAConstituency master_mlaconstituency = null;

                if (mlaconstituencyDetails != null)
                {

                    master_mlaconstituency = new MLAConstituency()
                    {

                        EncryptedMLAConstituencyCode = URLEncrypt.EncryptParameters1(new string[] { "MLAConstituencyCode =" + mlaconstituencyDetails.MAST_MLA_CONST_CODE.ToString() }),
                        MAST_MLA_CONST_NAME = mlaconstituencyDetails.MAST_MLA_CONST_NAME,
                        MAST_STATE_CODE = mlaconstituencyDetails.MAST_STATE_CODE
                    };
                }

                return master_mlaconstituency;

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

        public bool UpdateMLAConstituencyDetailsDAL(MLAConstituency master_mlaconstituency, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int mlaconstituencyCode = 0;
                encryptedParameters = master_mlaconstituency.EncryptedMLAConstituencyCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                mlaconstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstituencyCode"].ToString());


                Int32 recordCount = dbContext.MASTER_MLA_CONSTITUENCY.Where(mla => mla.MAST_MLA_CONST_NAME.ToUpper() == master_mlaconstituency.MAST_MLA_CONST_NAME.ToUpper() && mla.MAST_STATE_CODE == master_mlaconstituency.MAST_STATE_CODE && mla.MAST_MLA_CONST_CODE != mlaconstituencyCode).Count();
                if (recordCount > 0)
                {
                    message = "MLA Constituency Name under selected state is already exist.";
                    return false;
                }

                Models.MASTER_MLA_CONSTITUENCY mlaconstituencyDetails = dbContext.MASTER_MLA_CONSTITUENCY.Where(mla => mla.MAST_MLA_CONST_CODE == mlaconstituencyCode).FirstOrDefault();

                mlaconstituencyDetails.MAST_MLA_CONST_NAME = master_mlaconstituency.MAST_MLA_CONST_NAME;

                //Added by abhishek kamble 27-nov-2013
                mlaconstituencyDetails.USERID = PMGSYSession.Current.UserId;
                mlaconstituencyDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(mlaconstituencyDetails).State = System.Data.Entity.EntityState.Modified;
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


        #endregion MLAConstituencyMasterDataEntry


        #region MPConstituencyMasterDataEntry

        public Array GetMPConstituencyDetailsListDAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var query = from mpconstituencyDetails in dbContext.MASTER_MP_CONSTITUENCY
                            join stateDetails in dbContext.MASTER_STATE
                            on mpconstituencyDetails.MASTER_STATE.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                            where mpconstituencyDetails.MAST_MP_CONST_ACTIVE == "Y" &&
                            stateDetails.MAST_STATE_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            (stateCode == 0 ? 1 : mpconstituencyDetails.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)
                            select new { mpconstituencyDetails.MAST_MP_CONST_CODE, mpconstituencyDetails.MAST_MP_CONST_NAME, stateDetails.MAST_STATE_NAME, mpconstituencyDetails.MAST_STATE_CODE, mpconstituencyDetails.MAST_LOCK_STATUS };


                totalRecords = query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "MPConstituencyName":
                                query = query.OrderBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateName":
                                query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }


                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MPConstituencyName":
                                query = query.OrderByDescending(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "StateName":
                                query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(mpconstituencydetails => new
                {
                    mpconstituencydetails.MAST_MP_CONST_CODE,
                    mpconstituencydetails.MAST_MP_CONST_NAME,
                    mpconstituencydetails.MAST_STATE_NAME,
                    mpconstituencydetails.MAST_STATE_CODE,
                    mpconstituencydetails.MAST_LOCK_STATUS

                }).ToArray();


                return result.Select(mpconstituencydetails => new
                {
                    cell = new[] {                         
                                mpconstituencydetails.MAST_MP_CONST_NAME.ToString().Trim()=="<Null>"? "NA":mpconstituencydetails.MAST_MP_CONST_NAME.ToString().Trim() , 
                                mpconstituencydetails.MAST_STATE_NAME.ToString().Trim(),      
                                URLEncrypt.EncryptParameters1(new string[] { "MPConstituencyCode =" + mpconstituencydetails.MAST_MP_CONST_CODE.ToString(), "StateCode =" + mpconstituencydetails.MAST_STATE_CODE.ToString(), "MPConstituencyName =" + mpconstituencydetails.MAST_MP_CONST_NAME.ToString(),"StateName ="+mpconstituencydetails.MAST_STATE_NAME.ToString() }),
                                URLEncrypt.EncryptParameters1(new string[] { "MPConstituencyCode =" + mpconstituencydetails.MAST_MP_CONST_CODE.ToString(), "StateCode =" + mpconstituencydetails.MAST_STATE_CODE.ToString(), "MPConstituencyName =" + mpconstituencydetails.MAST_MP_CONST_NAME.ToString(),"StateName ="+mpconstituencydetails.MAST_STATE_NAME.ToString() }),
                                (PMGSYSession.Current.RoleCode == 23 || PMGSYSession.Current.RoleCode == 36) ?URLEncrypt.EncryptParameters1(new string[] { "MPConstituencyCode =" + mpconstituencydetails.MAST_MP_CONST_CODE.ToString()}):mpconstituencydetails.MAST_LOCK_STATUS=="N"?URLEncrypt.EncryptParameters1(new string[] { "MPConstituencyCode =" + mpconstituencydetails.MAST_MP_CONST_CODE.ToString()}):string.Empty
                                
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

        public bool SaveMPConstituencyDetailsDAL(MPConstituency master_mpconstituency, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Int32 recordCount = dbContext.MASTER_MP_CONSTITUENCY.Where(mpconstituency => mpconstituency.MAST_STATE_CODE == master_mpconstituency.MAST_STATE_CODE && mpconstituency.MAST_MP_CONST_NAME.ToUpper() == master_mpconstituency.MAST_MP_CONST_NAME.ToUpper()).Count();
                if (recordCount > 0)
                {
                    message = "MP Constituency Name under selected state is already exist.";
                    return false;
                }
                Models.MASTER_MP_CONSTITUENCY mpconstituencyDetails = new Models.MASTER_MP_CONSTITUENCY();


                mpconstituencyDetails.MAST_MP_CONST_CODE = (Int32)GetMaxCode(MasterDataEntryModules.MPConstituency);
                mpconstituencyDetails.MAST_MP_CONST_NAME = master_mpconstituency.MAST_MP_CONST_NAME;
                mpconstituencyDetails.MAST_STATE_CODE = master_mpconstituency.MAST_STATE_CODE;

                mpconstituencyDetails.MAST_MP_CONST_ACTIVE = "Y";
                mpconstituencyDetails.MAST_LOCK_STATUS = "N";

                //Added by abhishek kamble 27-nov-2013
                mpconstituencyDetails.USERID = PMGSYSession.Current.UserId;
                mpconstituencyDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_MP_CONSTITUENCY.Add(mpconstituencyDetails);
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


        public MPConstituency GetMPConstituencyDetailsDAL_ByMPConstituencyCode(int mpConstituencyCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_MP_CONSTITUENCY mpconstituencyDetails = dbContext.MASTER_MP_CONSTITUENCY.Where(mp => mp.MAST_MP_CONST_CODE == mpConstituencyCode).FirstOrDefault();

                MPConstituency master_mpconstituency = null;

                if (mpconstituencyDetails != null)
                {

                    master_mpconstituency = new MPConstituency()
                    {

                        EncryptedMpConstituencyCode = URLEncrypt.EncryptParameters1(new string[] { "MPConstituencyCode =" + mpconstituencyDetails.MAST_MP_CONST_CODE.ToString() }),
                        MAST_MP_CONST_NAME = mpconstituencyDetails.MAST_MP_CONST_NAME,
                        MAST_STATE_CODE = mpconstituencyDetails.MAST_STATE_CODE
                    };
                }

                return master_mpconstituency;

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

        public bool UpdateMPConstituencyDetailsDAL(MPConstituency master_mpconstituency, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int mpconstituencyCode = 0;
                encryptedParameters = master_mpconstituency.EncryptedMpConstituencyCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                mpconstituencyCode = Convert.ToInt32(decryptedParameters["MPConstituencyCode"].ToString());


                Int32 recordCount = dbContext.MASTER_MP_CONSTITUENCY.Where(mp => mp.MAST_MP_CONST_NAME.ToUpper() == master_mpconstituency.MAST_MP_CONST_NAME.ToUpper() && mp.MAST_STATE_CODE == master_mpconstituency.MAST_STATE_CODE && mp.MAST_MP_CONST_CODE != mpconstituencyCode).Count();
                if (recordCount > 0)
                {
                    message = "MP Constituency Name under selected state is already exist.";
                    return false;
                }

                Models.MASTER_MP_CONSTITUENCY mpconstituencyDetails = dbContext.MASTER_MP_CONSTITUENCY.Where(mp => mp.MAST_MP_CONST_CODE == mpconstituencyCode).FirstOrDefault();

                mpconstituencyDetails.MAST_MP_CONST_NAME = master_mpconstituency.MAST_MP_CONST_NAME;

                //Added by abhishek kamble 27-nov-2013
                mpconstituencyDetails.USERID = PMGSYSession.Current.UserId;
                mpconstituencyDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(mpconstituencyDetails).State = System.Data.Entity.EntityState.Modified;
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

        #endregion MPConstituencyMasterDataEntry


        #region OtherHabitationDetailsMasterDataEntry
        public Array GetOtherHabitationDetailsListDAL(int habitationCode, string lockStatus, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Int32 PMGSYYear = (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011);


                //Commented By Abhishek kamble 2-May-2014
                //var query = from otherHabitationDetails in dbContext.MASTER_HABITATIONS_DETAILS
                //            join habitationDetails in dbContext.MASTER_HABITATIONS
                //            on otherHabitationDetails.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                //            where otherHabitationDetails.MAST_HAB_CODE == habitationCode && otherHabitationDetails.MAST_YEAR == PMGSYYear
                //            select new
                //            {
                //                otherHabitationDetails.MAST_HAB_CODE,
                //                habitationDetails.MAST_HAB_NAME,
                //                otherHabitationDetails.MAST_YEAR,
                //                otherHabitationDetails.MAST_HAB_TOT_POP,
                //                otherHabitationDetails.MAST_HAB_SCST_POP,
                //                //Added By Abhishek Kamble 14-Mar-2014
                //                otherHabitationDetails.MAST_LOCK_STATUS,
                //                otherHabitationDetails.MAST_HAB_CONNECTED,
                //                otherHabitationDetails.MAST_SCHEME
                //            };

                //Added By Abhishek kamble 2-May-2014
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();  //added byy pp 23-11-2017
                var query = dbContext.USP_MAS_LIST_HAB_DETAILS(habitationCode, PMGSYYear, roleCode).ToList(); //(short)PMGSYSession.Current.RoleCode


                totalRecords = query == null ? 0 : query.Count();

                // query = query.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "HabitationName":
                                query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Year":
                                query = query.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "TotalPopulation":
                                query = query.OrderBy(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "SCSTPopulation":
                                query = query.OrderBy(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LockStatus":
                                query = query.OrderBy(x => x.UNLOCK_BY_MORD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }


                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "HabitationName":
                                query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Year":
                                query = query.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "TotalPopulation":
                                query = query.OrderByDescending(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "SCSTPopulation":
                                query = query.OrderByDescending(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LockStatus":
                                query = query.OrderByDescending(x => x.UNLOCK_BY_MORD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = query.Select(habitationDetails => new
                {
                    habitationDetails.MAST_HAB_CODE,
                    habitationDetails.MAST_HAB_NAME,
                    habitationDetails.MAST_YEAR,
                    habitationDetails.MAST_HAB_TOT_POP,
                    habitationDetails.MAST_HAB_SCST_POP,
                    //Added By Abhishek kamble 14-Mar-2014               
                    habitationDetails.UNLOCK_BY_MORD,
                    habitationDetails.MAST_HAB_CONNECTED,
                    habitationDetails.MAST_SCHEME,
                }).ToArray();


                return result.Select(habitationDetails => new
                {
                    cell = new[] {                         

                                habitationDetails.MAST_HAB_NAME.Trim() == string.Empty? "NA": habitationDetails.MAST_HAB_NAME.Trim(),                                                                
                            //Added By Abhishek kamble 14-Mar-2014               
                                habitationDetails.MAST_HAB_CONNECTED=="Y"?"Yes":"No",
                                habitationDetails.MAST_SCHEME.ToUpper()=="P"?"Yes":"No",
                                habitationDetails.MAST_YEAR.ToString().Trim() ,                                
                                /*habitationDetails.MAST_HAB_TOT_POP == null ? "NA": habitationDetails.MAST_HAB_TOT_POP.ToString(),
                                habitationDetails.MAST_HAB_SCST_POP == null ? "NA": habitationDetails.MAST_HAB_SCST_POP.ToString(),  */     
                                habitationDetails.MAST_HAB_TOT_POP.ToString(),
                                habitationDetails.MAST_HAB_SCST_POP.ToString(),  
                               //(habitationDetails.UNLOCK_BY_MORD=="N")?
                               //"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Other Habitation Details' onClick ='EditOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Other Details' onClick ='DeleteHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td><td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Other Habitation Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td></tr></table></center>":
                               //"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Habitation Other Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td></tr></table></center>"   //Added By Abhishek kamble 14-Mar-2014
                                    PMGSYSession.Current.RoleCode == 23 ?
                                    "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Other Habitation Details' onClick ='EditOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Other Details' onClick ='DeleteHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td><td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Other Habitation Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td></tr></table></center>"
                                    :(habitationDetails.UNLOCK_BY_MORD=="M")?
                                    "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked' title='Edit Other Habitation Details' onClick ='EditOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-unlocked' title='Delete Habitation Other Details' onClick ='DeleteHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td><td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Other Habitation Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td></tr></table></center>"
                                     :(habitationDetails.UNLOCK_BY_MORD=="N")?
                                    "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Other Habitation Details' onClick ='EditOtherHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td> <td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Habitation Other Details' onClick ='DeleteHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td><td  style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Other Habitation Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() })+ "\");'></span></td></tr></table></center>"
                                    :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td  style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td><td style='border:none'><span class='ui-icon ui-icon-zoomin' title='View Habitation Other Details' onClick ='ViewHabitationOtherDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(),"Year =" + habitationDetails.MAST_YEAR.ToString(),"HabitationName =" + habitationDetails.MAST_HAB_NAME.ToString() }) + "\");'></span></td></tr></table></center>",
                                    habitationDetails.UNLOCK_BY_MORD=="N"?"No":habitationDetails.UNLOCK_BY_MORD=="M"?"Unlock":"Yes"        
                             }
                }).ToArray();

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetOtherHabitationDetailsListDAL()");
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


        public bool SaveOtherHabitationDetailsDAL(HabitationDetails details_habitations, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            details_habitations.YearID = (PMGSYSession.Current.PMGSYScheme == 1 ? Convert.ToInt16(2001) : Convert.ToInt16(2011));
            try
            {
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {

                    int habitationCode = 0;
                    encryptedParameters = details_habitations.EncryptedHabitationCode_OtherDetails.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());



                    Int32 recordCount = dbContext.MASTER_HABITATIONS_DETAILS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode && habitation.MAST_YEAR == details_habitations.YearID).Count();
                    // Int32 recordCount = dbContext.MASTER_HABITATIONS_DETAILS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode && habitation.MAST_YEAR == Convert.ToInt32(details_habitations.Years)).Count();
                    if (recordCount > 0)
                    {
                        message = "Habitation Details under selected year is already exist.";
                        return false;
                    }

                    if (!CheckPopulation(true, dbContext, habitationCode, details_habitations, ref message))
                    {
                        return false;
                    }

                    using (var scope = new TransactionScope())
                    {
                        Models.MASTER_HABITATIONS_DETAILS habitationDetails = new Models.MASTER_HABITATIONS_DETAILS();

                        habitationDetails.MAST_HAB_CODE = habitationCode;
                        habitationDetails.MAST_YEAR = details_habitations.YearID;
                        habitationDetails.MAST_HAB_TOT_POP = details_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_TOT_POP;
                        habitationDetails.MAST_HAB_SCST_POP = details_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_SCST_POP;

                        habitationDetails.MAST_HAB_CONNECTED = details_habitations.HasHabConnected == true ? "Y" : "N";
                        habitationDetails.MAST_PANCHAYAT_HQ = details_habitations.ISPanchayatHQ == true ? "Y" : "N";
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "Y" : "N";
                        //Modified By Abhishek kamble 29-Apr-2014
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "P" : "O";
                        habitationDetails.MAST_SCHEME = (details_habitations.HasHabConnected == false ? "N" : details_habitations.ISScheme == true ? "P" : "O");

                        habitationDetails.MAST_PRIMARY_SCHOOL = details_habitations.HasPrimarySchool == true ? "Y" : "N";
                        habitationDetails.MAST_MIDDLE_SCHOOL = details_habitations.HasMiddleSchool == true ? "Y" : "N";
                        habitationDetails.MAST_HIGH_SCHOOL = details_habitations.HasHighSchool == true ? "Y" : "N";
                        habitationDetails.MAST_INTERMEDIATE_SCHOOL = details_habitations.HasIntermediateSchool == true ? "Y" : "N";
                        habitationDetails.MAST_DEGREE_COLLEGE = details_habitations.HasDegreeCollege == true ? "Y" : "N";
                        habitationDetails.MAST_HEALTH_SERVICE = details_habitations.HasHealthService == true ? "Y" : "N";
                        habitationDetails.MAST_DISPENSARY = details_habitations.HasDespensary == true ? "Y" : "N";
                        habitationDetails.MAST_PHCS = details_habitations.HasPHCS == true ? "Y" : "N";
                        habitationDetails.MAST_VETNARY_HOSPITAL = details_habitations.HasVetnaryHospital == true ? "Y" : "N";
                        habitationDetails.MAST_MCW_CENTERS = details_habitations.HasMCWCenters == true ? "Y" : "N";
                        habitationDetails.MAST_TELEGRAPH_OFFICE = details_habitations.HasTelegraphOffice == true ? "Y" : "N";
                        habitationDetails.MAST_TELEPHONE_CONNECTION = details_habitations.HasTelephoneConnection == true ? "Y" : "N";
                        ///Added for Mining Field
                        habitationDetails.MAST_MINING = "N";
                        habitationDetails.MAST_BUS_SERVICE = details_habitations.HasBusService == true ? "Y" : "N";
                        habitationDetails.MAST_RAILWAY_STATION = details_habitations.HasRailwayStation == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICTY = details_habitations.HasElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_TOURIST_PLACE = details_habitations.IsTouristPlace == true ? "Y" : "N";
                        habitationDetails.MAST_LOCK_STATUS = "N";

                        habitationDetails.MAST_HAB_WEIGHTAGE = 0.00M;
                        habitationDetails.MAST_ITI = "N";
                        habitationDetails.MAST_PETROL_PUMP = "N";
                        habitationDetails.MAST_PUMP_ADD = "N";
                        habitationDetails.MAST_ELECTRICITY_ADD = "N";
                        habitationDetails.MAST_MANDI = "N";
                        habitationDetails.MAST_WAREHOUSE = "N";
                        habitationDetails.MAST_RETAIL_SHOP = "N";
                        habitationDetails.MAST_SUB_TEHSIL = "N";
                        habitationDetails.MAST_BLOCK_HQ = "N";

                        //added by abhishek kamble 27-nov-2013
                        habitationDetails.USERID = PMGSYSession.Current.UserId;
                        habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MASTER_HABITATIONS_DETAILS.Add(habitationDetails);


                        Models.MASTER_HABITATIONS habitationMaster = dbContext.MASTER_HABITATIONS.Where(hm => hm.MAST_HAB_CODE == habitationCode).FirstOrDefault();


                        if (habitationMaster != null && (habitationMaster.MAST_HAB_STATUS == "U" || habitationMaster.MAST_HAB_STATUS == "C"))
                        {
                            habitationMaster.MAST_HAB_STATUS = details_habitations.HasHabConnected == true ? "C" : "U";

                            //added by abhishek kamble 27-nov-2013
                            habitationMaster.USERID = PMGSYSession.Current.UserId;
                            habitationMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.Entry(habitationMaster).State = System.Data.Entity.EntityState.Modified;
                        }

                        //Time Out Added By Abhishek kamble 24-Feb-2014
                        ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        dbContext.SaveChanges();
                        scope.Complete();
                    }

                    return true;
                }

                 //added by ujjwal saket on19-10-2013 for PMGSY-II
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    int habitationCode = 0;
                    encryptedParameters = details_habitations.EncryptedHabitationCode_OtherDetails.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());

                    Int32 recordCount = dbContext.MASTER_HABITATIONS_DETAILS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode && habitation.MAST_YEAR == details_habitations.YearID).Count();
                    if (recordCount > 0)
                    {
                        message = "Habitation Details under selected year is already exist.";
                        return false;
                    }

                    if (!CheckPopulation(true, dbContext, habitationCode, details_habitations, ref message))
                    {
                        return false;
                    }

                    using (var scope = new TransactionScope())
                    {
                        Models.MASTER_HABITATIONS_DETAILS habitationDetails = new Models.MASTER_HABITATIONS_DETAILS();

                        habitationDetails.MAST_HAB_CODE = habitationCode;
                        habitationDetails.MAST_YEAR = details_habitations.YearID;
                        habitationDetails.MAST_HAB_TOT_POP = details_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_TOT_POP;
                        habitationDetails.MAST_HAB_SCST_POP = details_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_SCST_POP;

                        habitationDetails.MAST_HAB_CONNECTED = details_habitations.HasHabConnected == true ? "Y" : "N";
                        habitationDetails.MAST_PANCHAYAT_HQ = details_habitations.ISPanchayatHQ == true ? "Y" : "N";
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "Y" : "N";

                        //Modified By Abhishek kamble 29-Apr-2014
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "P" : "O";
                        habitationDetails.MAST_SCHEME = (details_habitations.HasHabConnected == false ? "N" : details_habitations.ISScheme == true ? "P" : "O");

                        habitationDetails.MAST_PRIMARY_SCHOOL = details_habitations.HasPrimarySchool == true ? "Y" : "N";
                        habitationDetails.MAST_MIDDLE_SCHOOL = details_habitations.HasMiddleSchool == true ? "Y" : "N";
                        habitationDetails.MAST_HIGH_SCHOOL = details_habitations.HasHighSchool == true ? "Y" : "N";
                        habitationDetails.MAST_INTERMEDIATE_SCHOOL = details_habitations.HasIntermediateSchool == true ? "Y" : "N";
                        habitationDetails.MAST_DEGREE_COLLEGE = details_habitations.HasDegreeCollege == true ? "Y" : "N";
                        habitationDetails.MAST_HEALTH_SERVICE = details_habitations.HasHealthService == true ? "Y" : "N";
                        habitationDetails.MAST_DISPENSARY = details_habitations.HasDespensary == true ? "Y" : "N";
                        habitationDetails.MAST_PHCS = details_habitations.HasPHCS == true ? "Y" : "N";
                        habitationDetails.MAST_VETNARY_HOSPITAL = details_habitations.HasVetnaryHospital == true ? "Y" : "N";
                        habitationDetails.MAST_MCW_CENTERS = details_habitations.HasMCWCenters == true ? "Y" : "N";
                        habitationDetails.MAST_TELEGRAPH_OFFICE = details_habitations.HasTelegraphOffice == true ? "Y" : "N";
                        habitationDetails.MAST_TELEPHONE_CONNECTION = details_habitations.HasTelephoneConnection == true ? "Y" : "N";
                        ///Added for Mining Field
                        habitationDetails.MAST_MINING = details_habitations.HasMining == true ? "Y" : "N";
                        habitationDetails.MAST_BUS_SERVICE = details_habitations.HasBusService == true ? "Y" : "N";
                        habitationDetails.MAST_RAILWAY_STATION = details_habitations.HasRailwayStation == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICTY = details_habitations.HasElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_TOURIST_PLACE = details_habitations.IsTouristPlace == true ? "Y" : "N";

                        habitationDetails.MAST_ITI = details_habitations.HasITI == true ? "Y" : "N";
                        habitationDetails.MAST_PETROL_PUMP = details_habitations.HasPetrolPump == true ? "Y" : "N";
                        habitationDetails.MAST_PUMP_ADD = details_habitations.HasAdditionalPetrolPump == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICITY_ADD = details_habitations.HasAdditionalElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_MANDI = details_habitations.HasMandi == true ? "Y" : "N";
                        habitationDetails.MAST_WAREHOUSE = details_habitations.HasWarehouse == true ? "Y" : "N";
                        habitationDetails.MAST_RETAIL_SHOP = details_habitations.HasRetailShop == true ? "Y" : "N";
                        habitationDetails.MAST_SUB_TEHSIL = details_habitations.HasSubTehsil == true ? "Y" : "N";
                        habitationDetails.MAST_BLOCK_HQ = details_habitations.HasBlockHeadquarter == true ? "Y" : "N";
                        habitationDetails.MAST_HAB_WEIGHTAGE = 0.00M;
                        habitationDetails.MAST_LOCK_STATUS = "N";




                        dbContext.MASTER_HABITATIONS_DETAILS.Add(habitationDetails);


                        Models.MASTER_HABITATIONS habitationMaster = dbContext.MASTER_HABITATIONS.Where(hm => hm.MAST_HAB_CODE == habitationCode).FirstOrDefault();


                        if (habitationMaster != null && (habitationMaster.MAST_HAB_STATUS == "U" || habitationMaster.MAST_HAB_STATUS == "C"))
                        {
                            habitationMaster.MAST_HAB_STATUS = details_habitations.HasHabConnected == true ? "C" : "U";
                            dbContext.Entry(habitationMaster).State = System.Data.Entity.EntityState.Modified;
                        }

                        //Time Out Added By Abhishek kamble 24-Feb-2014
                        ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        dbContext.SaveChanges();
                        scope.Complete();
                    }
                    return true;


                }
                return false;
                //finish addtion
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveOtherHabitationDetailsDAL().OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveOtherHabitationDetailsDAL().UpdateException");
                return false;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveOtherHabitationDetailsDAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                message = new CommonFunctions().FormatErrorMessage(modelstate);

                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveOtherHabitationDetailsDAL()");
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

        private bool CheckPopulation(bool isAdd, Models.PMGSYEntities dbContext, int habitationCode, HabitationDetails details_habitations, ref string message)
        {
            try
            {
                int? totalPopulation = 0;
                int? SCSTPopulation = 0;

                Int32 PMGSYYear = (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011);

                //change by Ujjwal Saket on 24-10-2013
                //Models.MASTER_VILLAGE villageMaster = (from villageDetails in dbContext.MASTER_VILLAGE
                //                                       join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION
                //                                       on villageDetails.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE
                //                                       where villageDetails.MAST_VILLAGE_CODE == dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).Select(mh => mh.MAST_VILLAGE_CODE).FirstOrDefault()
                //                                       && villagePopulation.MAST_CENSUS_YEAR==PMGSYYear
                //                                       select villageDetails).FirstOrDefault();


                //change by Abhishek kamble on 11-feb-2014
                Models.MASTER_VILLAGE_POPULATION villagePopulationMaster = (from villageDetails in dbContext.MASTER_VILLAGE
                                                                            join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION
                                                                            on villageDetails.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE
                                                                            where villageDetails.MAST_VILLAGE_CODE == dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).Select(mh => mh.MAST_VILLAGE_CODE).FirstOrDefault()
                                                                            && villagePopulation.MAST_CENSUS_YEAR == PMGSYYear
                                                                            select villagePopulation).FirstOrDefault();

                //Models.MASTER_VILLAGE villageMaster = (from villageDetails in dbContext.MASTER_VILLAGE

                //                                       where villageDetails.MAST_VILLAGE_CODE == dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).Select(mh => mh.MAST_VILLAGE_CODE).FirstOrDefault()

                //                                       select villageDetails).FirstOrDefault();

                if (villagePopulationMaster != null)
                {


                    if (isAdd)
                    {
                        totalPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habMaster in dbContext.MASTER_HABITATIONS
                                           on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                           where
                                           habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE && habDetails.MAST_YEAR == PMGSYYear
                                           && habMaster.MAST_HABITATION_ACTIVE == "Y" // Added on 11 Aug 2020. Suggested By Srinivasa Sir.
                                           select habDetails
                                              ).Sum(hd => (Int32?)hd.MAST_HAB_TOT_POP);



                        SCSTPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                          join habMaster in dbContext.MASTER_HABITATIONS
                                          on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                          where
                                          habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE && habDetails.MAST_YEAR == PMGSYYear
                                          && habMaster.MAST_HABITATION_ACTIVE == "Y" // Added on 11 Aug 2020. Suggested By Srinivasa Sir.
                                          select habDetails
                                             ).Sum(hd => (Int32?)hd.MAST_HAB_SCST_POP);
                    }
                    else
                    {
                        totalPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habMaster in dbContext.MASTER_HABITATIONS
                                           on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                           where
                                           habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE &&
                                           habDetails.MAST_HAB_CODE != habitationCode && habDetails.MAST_YEAR == PMGSYYear
                                           && habMaster.MAST_HABITATION_ACTIVE == "Y" // Added on 11 Aug 2020. Suggested By Srinivasa Sir.
                                           select habDetails
                                            ).Sum(hd => (Int32?)hd.MAST_HAB_TOT_POP);

                        SCSTPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                          join habMaster in dbContext.MASTER_HABITATIONS
                                          on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                          where
                                          habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE &&
                                          habDetails.MAST_HAB_CODE != habitationCode && habDetails.MAST_YEAR == PMGSYYear
                                           && habMaster.MAST_HABITATION_ACTIVE == "Y" // Added on 11 Aug 2020. Suggested By Srinivasa Sir.
                                          select habDetails
                                            ).Sum(hd => (Int32?)hd.MAST_HAB_SCST_POP);
                    }

                    totalPopulation = (totalPopulation == null ? 0 : totalPopulation) + (Int32)(details_habitations.MAST_HAB_TOT_POP == null ? 0 : details_habitations.MAST_HAB_TOT_POP);
                    SCSTPopulation = (SCSTPopulation == null ? 0 : SCSTPopulation) + (Int32)(details_habitations.MAST_HAB_SCST_POP == null ? 0 : details_habitations.MAST_HAB_SCST_POP);

                    //Modified by abhishek kamble form PMGSY II changes 5-feb-2014
                    Int64 ChangedVillagePopulation = (villagePopulationMaster.MAST_VILLAGE_TOT_POP + ((villagePopulationMaster.MAST_VILLAGE_TOT_POP * 20) / 100));
                    //if (totalPopulation > villageMaster.MAST_VILLAGE_TOT_POP && SCSTPopulation > villageMaster.MAST_VILLAGE_SCST_POP)                                       
                    if (totalPopulation > ChangedVillagePopulation && SCSTPopulation > villagePopulationMaster.MAST_VILLAGE_SCST_POP)
                    {
                        message = "<ul><li>Total Population of all habitations under village should be less than total population of village which is " + ChangedVillagePopulation + " .</li><li>SCST Population of all habitations under selected village should be less than SCST population of village which is " + villagePopulationMaster.MAST_VILLAGE_SCST_POP + ".</li></ul>";
                        return false;
                    }
                    // else if (totalPopulation > villageMaster.MAST_VILLAGE_TOT_POP)
                    else if (totalPopulation > ChangedVillagePopulation)
                    {
                        message = "Total Population of all habitations under village should be less than total population of village which is " + ChangedVillagePopulation + ".";
                        return false;
                    }
                    else if (SCSTPopulation > villagePopulationMaster.MAST_VILLAGE_SCST_POP)
                    {
                        message = "SCST Population of all habitations under village should be less than SCST population of village which is " + villagePopulationMaster.MAST_VILLAGE_SCST_POP + ".";
                        return false;
                    }


                }
                else
                {           //Added By Abhishek kamble 5-feb-2014
                    message = "Habitation details not saved because village population details are not entered.";
                    return false;
                }


                return true;


            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "CheckPopulation().DAL");
                return false;
            }
            //Not Dispose dbContext here
            //finally  
            //{
            //    if (dbContext != null)
            //    {
            //        dbContext.Dispose();    //Not Dispose dbContext here
            //    }
            //}
        }

        //Added by Abhishek kamble 24-Feb-2014    
        public bool CheckRemainingPopulation(bool isAdd, Models.PMGSYEntities dbContext, int habitationCode, ref int totalRemainingPopulation, ref int totalRemainingSCSTPopulation, ref int totalVillagePopulation, ref Int64 totalVillagePopulation20Per, ref int totalVillageSCSTPopulation, ref string message)
        {
            try
            {
                int? totalPopulation = 0;
                int? SCSTPopulation = 0;

                Int32 PMGSYYear = (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011);

                Models.MASTER_VILLAGE_POPULATION villagePopulationMaster = (from villageDetails in dbContext.MASTER_VILLAGE
                                                                            join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION
                                                                            on villageDetails.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE
                                                                            where villageDetails.MAST_VILLAGE_CODE == dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).Select(mh => mh.MAST_VILLAGE_CODE).FirstOrDefault()
                                                                            && villagePopulation.MAST_CENSUS_YEAR == PMGSYYear
                                                                            select villagePopulation).FirstOrDefault();
                if (villagePopulationMaster != null)
                {


                    if (isAdd)
                    {
                        totalPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habMaster in dbContext.MASTER_HABITATIONS
                                           on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                           where
                                           habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE && habDetails.MAST_YEAR == PMGSYYear

                                           select habDetails
                                              ).Sum(hd => (Int32?)hd.MAST_HAB_TOT_POP);



                        SCSTPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                          join habMaster in dbContext.MASTER_HABITATIONS
                                          on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                          where
                                          habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE && habDetails.MAST_YEAR == PMGSYYear
                                          select habDetails
                                             ).Sum(hd => (Int32?)hd.MAST_HAB_SCST_POP);
                    }
                    else
                    {
                        totalPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habMaster in dbContext.MASTER_HABITATIONS
                                           on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                           where
                                           habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE &&
                                           habDetails.MAST_HAB_CODE != habitationCode && habDetails.MAST_YEAR == PMGSYYear
                                           select habDetails
                                            ).Sum(hd => (Int32?)hd.MAST_HAB_TOT_POP);

                        SCSTPopulation = (from habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                          join habMaster in dbContext.MASTER_HABITATIONS
                                          on habDetails.MAST_HAB_CODE equals habMaster.MAST_HAB_CODE
                                          where
                                          habMaster.MAST_VILLAGE_CODE == villagePopulationMaster.MAST_VILLAGE_CODE &&
                                          habDetails.MAST_HAB_CODE != habitationCode && habDetails.MAST_YEAR == PMGSYYear
                                          select habDetails
                                            ).Sum(hd => (Int32?)hd.MAST_HAB_SCST_POP);
                    }

                    totalPopulation = (totalPopulation == null ? 0 : totalPopulation);
                    SCSTPopulation = (SCSTPopulation == null ? 0 : SCSTPopulation);

                    Int64 ChangedVillagePopulation = (villagePopulationMaster.MAST_VILLAGE_TOT_POP + ((villagePopulationMaster.MAST_VILLAGE_TOT_POP * 20) / 100));

                    totalRemainingPopulation = Convert.ToInt32(ChangedVillagePopulation - totalPopulation);
                    totalRemainingSCSTPopulation = Convert.ToInt32(villagePopulationMaster.MAST_VILLAGE_SCST_POP - SCSTPopulation);

                    totalRemainingPopulation = totalRemainingPopulation < 0 ? 0 : totalRemainingPopulation;
                    totalRemainingSCSTPopulation = totalRemainingSCSTPopulation < 0 ? 0 : totalRemainingSCSTPopulation;

                    //Added By Abhishek kamble 14-Mar-2014
                    totalVillagePopulation = villagePopulationMaster.MAST_VILLAGE_TOT_POP;
                    totalVillageSCSTPopulation = villagePopulationMaster.MAST_VILLAGE_SCST_POP;
                    totalVillagePopulation20Per = ChangedVillagePopulation;

                    return true;
                }
                else
                {
                    message = "Please enter village population details first.";
                    return false;
                }

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "CheckRemainingPopulation().DAL");
                return false;
            }
            //finally
            //{
            //    if (dbContext != null)
            //    {
            //        dbContext.Dispose();//Not Dispose dbContext here
            //    }
            //}
        }


        public HabitationDetails GetOtherHabitationDetailsDAL_ByHabitationCodeandYear(int habitationCode, short year)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.PMGSYScheme == 1 && year == 2001)
                {

                    Models.MASTER_HABITATIONS_DETAILS habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Where(h => h.MAST_HAB_CODE == habitationCode && h.MAST_YEAR == year).FirstOrDefault();

                    HabitationDetails details_habitation = null;

                    if (habitationDetails != null)
                    {

                        details_habitation = new HabitationDetails()
                        {
                            EncryptedHabitationCode_OtherDetails = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString() }),
                            EncryptedHabitationDetailsCode = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "Year =" + habitationDetails.MAST_YEAR.ToString() }),
                            YearID = (Int16)habitationDetails.MAST_YEAR,
                            MAST_HAB_TOT_POP = habitationDetails.MAST_HAB_TOT_POP,
                            MAST_HAB_SCST_POP = habitationDetails.MAST_HAB_SCST_POP,
                            HasHabConnected = habitationDetails.MAST_HAB_CONNECTED == "Y" ? true : false,
                            ISScheme = habitationDetails.MAST_SCHEME == "P" ? true : false,
                            HasPrimarySchool = habitationDetails.MAST_PRIMARY_SCHOOL == "Y" ? true : false,
                            HasMiddleSchool = habitationDetails.MAST_MIDDLE_SCHOOL == "Y" ? true : false,
                            HasHighSchool = habitationDetails.MAST_HIGH_SCHOOL == "Y" ? true : false,
                            HasIntermediateSchool = habitationDetails.MAST_INTERMEDIATE_SCHOOL == "Y" ? true : false,
                            HasDegreeCollege = habitationDetails.MAST_DEGREE_COLLEGE == "Y" ? true : false,
                            HasHealthService = habitationDetails.MAST_HEALTH_SERVICE == "Y" ? true : false,
                            HasDespensary = habitationDetails.MAST_DISPENSARY == "Y" ? true : false,
                            HasMCWCenters = habitationDetails.MAST_MCW_CENTERS == "Y" ? true : false,
                            HasPHCS = habitationDetails.MAST_PHCS == "Y" ? true : false,
                            HasVetnaryHospital = habitationDetails.MAST_VETNARY_HOSPITAL == "Y" ? true : false,
                            HasTelegraphOffice = habitationDetails.MAST_TELEGRAPH_OFFICE == "Y" ? true : false,
                            HasTelephoneConnection = habitationDetails.MAST_TELEPHONE_CONNECTION == "Y" ? true : false,
                            HasBusService = habitationDetails.MAST_BUS_SERVICE == "Y" ? true : false,
                            HasRailwayStation = habitationDetails.MAST_RAILWAY_STATION == "Y" ? true : false,
                            HasElectricity = habitationDetails.MAST_ELECTRICTY == "Y" ? true : false,
                            ISPanchayatHQ = habitationDetails.MAST_PANCHAYAT_HQ == "Y" ? true : false,
                            IsTouristPlace = habitationDetails.MAST_TOURIST_PLACE == "Y" ? true : false

                        };
                    }

                    return details_habitation;
                }
                else
                {
                    Models.MASTER_HABITATIONS_DETAILS habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Where(h => h.MAST_HAB_CODE == habitationCode && h.MAST_YEAR == year).FirstOrDefault();

                    HabitationDetails details_habitation = null;

                    if (habitationDetails != null)
                    {

                        details_habitation = new HabitationDetails()
                        {
                            EncryptedHabitationCode_OtherDetails = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString() }),
                            EncryptedHabitationDetailsCode = URLEncrypt.EncryptParameters1(new string[] { "HabitationCode =" + habitationDetails.MAST_HAB_CODE.ToString(), "Year =" + habitationDetails.MAST_YEAR.ToString() }),
                            YearID = (Int16)habitationDetails.MAST_YEAR,
                            MAST_HAB_TOT_POP = habitationDetails.MAST_HAB_TOT_POP,
                            MAST_HAB_SCST_POP = habitationDetails.MAST_HAB_SCST_POP,
                            HasHabConnected = habitationDetails.MAST_HAB_CONNECTED == "Y" ? true : false,
                            ISScheme = habitationDetails.MAST_SCHEME == "P" ? true : false,
                            HasPrimarySchool = habitationDetails.MAST_PRIMARY_SCHOOL == "Y" ? true : false,
                            HasMiddleSchool = habitationDetails.MAST_MIDDLE_SCHOOL == "Y" ? true : false,
                            HasHighSchool = habitationDetails.MAST_HIGH_SCHOOL == "Y" ? true : false,
                            HasIntermediateSchool = habitationDetails.MAST_INTERMEDIATE_SCHOOL == "Y" ? true : false,
                            HasITI = habitationDetails.MAST_ITI == "Y" ? true : false,
                            HasDegreeCollege = habitationDetails.MAST_DEGREE_COLLEGE == "Y" ? true : false,
                            HasHealthService = habitationDetails.MAST_HEALTH_SERVICE == "Y" ? true : false,
                            HasPHCS = habitationDetails.MAST_PHCS == "Y" ? true : false,
                            HasMCWCenters = habitationDetails.MAST_MCW_CENTERS == "Y" ? true : false,
                            HasDespensary = habitationDetails.MAST_DISPENSARY == "Y" ? true : false,
                            HasVetnaryHospital = habitationDetails.MAST_VETNARY_HOSPITAL == "Y" ? true : false,
                            HasRailwayStation = habitationDetails.MAST_RAILWAY_STATION == "Y" ? true : false,
                            HasBusService = habitationDetails.MAST_BUS_SERVICE == "Y" ? true : false,
                            IsTouristPlace = habitationDetails.MAST_TOURIST_PLACE == "Y" ? true : false,
                            HasTelegraphOffice = habitationDetails.MAST_TELEGRAPH_OFFICE == "Y" ? true : false,
                            HasTelephoneConnection = habitationDetails.MAST_TELEPHONE_CONNECTION == "Y" ? true : false,
                            ///Added for Mining Field
                            HasMining = habitationDetails.MAST_MINING == "Y" ? true : false,
                            HasPetrolPump = habitationDetails.MAST_PETROL_PUMP == "Y" ? true : false,
                            HasAdditionalPetrolPump = habitationDetails.MAST_PUMP_ADD == "Y" ? true : false,
                            HasElectricity = habitationDetails.MAST_ELECTRICTY == "Y" ? true : false,
                            HasAdditionalElectricity = habitationDetails.MAST_ELECTRICITY_ADD == "Y" ? true : false,
                            HasMandi = habitationDetails.MAST_MANDI == "Y" ? true : false,
                            HasWarehouse = habitationDetails.MAST_WAREHOUSE == "Y" ? true : false,
                            HasRetailShop = habitationDetails.MAST_RETAIL_SHOP == "Y" ? true : false,
                            ISPanchayatHQ = habitationDetails.MAST_PANCHAYAT_HQ == "Y" ? true : false,
                            HasSubTehsil = habitationDetails.MAST_SUB_TEHSIL == "Y" ? true : false,
                            HasBlockHeadquarter = habitationDetails.MAST_BLOCK_HQ == "Y" ? true : false


                        };
                    }

                    return details_habitation;

                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetOtherHabitationDetailsDAL_ByHabitationCodeandYear()");
                return null;
            }
            //finally
            //{
            //    if (dbContext != null)
            //    {
            //        dbContext.Dispose();//Not Dispose dbContext here
            //    }
            //}
        }

        public bool UpdateOtherHabitationDetailsDAL(HabitationDetails details_habitations, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    int habitationCode = 0;
                    int year = 0;
                    encryptedParameters = details_habitations.EncryptedHabitationDetailsCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());
                    year = Convert.ToInt32(decryptedParameters["Year"].ToString());

                    if (!CheckPopulation(false, dbContext, habitationCode, details_habitations, ref message))
                    {
                        return false;
                    }

                    Models.MASTER_HABITATIONS_DETAILS habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode && habitation.MAST_YEAR == year).FirstOrDefault();

                    //check applied if change status unconnected to connected habitaion when edit

                    if (habitationDetails.MAST_HAB_CONNECTED.Equals("N") && details_habitations.HasHabConnected)
                    {

                        int count = (from benefitedHabs in dbContext.IMS_BENEFITED_HABS
                                     join IMS in dbContext.IMS_SANCTIONED_PROJECTS
                                     on benefitedHabs.IMS_PR_ROAD_CODE equals IMS.IMS_PR_ROAD_CODE
                                     where
                                     benefitedHabs.MAST_HAB_CODE == habitationCode &&
                                     IMS.IMS_UPGRADE_CONNECT == "N"
                                     select IMS.IMS_PR_ROAD_CODE).Count();

                        if (count > 0)
                        {
                            message = "Habitation has been mapped to new type of proposal, so you can not update connectivity status to connected.";
                            return false;
                        }
                    }

                    //commented by Vikram as suggested by Srinivasa sir on 16-Apr-2015
                    //if (dbContext.IMS_BENEFITED_HABS.Any(bh => bh.MAST_HAB_CODE == habitationCode))
                    //{
                    //    var lstProposals = dbContext.IMS_BENEFITED_HABS.Where(m => m.MAST_HAB_CODE == habitationCode).Select(m => m.IMS_PR_ROAD_CODE);

                    //    if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => lstProposals.Contains(m.IMS_PR_ROAD_CODE) && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    //    {
                    //        if (habitationDetails.MAST_HAB_TOT_POP != details_habitations.MAST_HAB_TOT_POP || habitationDetails.MAST_HAB_SCST_POP != details_habitations.MAST_HAB_SCST_POP)
                    //        {
                    //            message = "Habitation has been mapped to the proposal, so you can not update population.";
                    //            return false;
                    //        }
                    //    }
                    //}


                    using (var scope = new TransactionScope())
                    {

                        habitationDetails.MAST_HAB_TOT_POP = details_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_TOT_POP;
                        habitationDetails.MAST_HAB_SCST_POP = details_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_SCST_POP;
                        habitationDetails.MAST_HAB_CONNECTED = details_habitations.HasHabConnected == true ? "Y" : "N";
                        habitationDetails.MAST_PANCHAYAT_HQ = details_habitations.ISPanchayatHQ == true ? "Y" : "N";
                        // habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "Y" : "N";

                        //Modified By Abhishek kamble 29-Apr-2014
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "P" : "O";

                        habitationDetails.MAST_SCHEME = (details_habitations.HasHabConnected == false ? "N" : details_habitations.ISScheme == true ? "P" : "O");

                        habitationDetails.MAST_PRIMARY_SCHOOL = details_habitations.HasPrimarySchool == true ? "Y" : "N";
                        habitationDetails.MAST_MIDDLE_SCHOOL = details_habitations.HasMiddleSchool == true ? "Y" : "N";
                        habitationDetails.MAST_HIGH_SCHOOL = details_habitations.HasHighSchool == true ? "Y" : "N";
                        habitationDetails.MAST_INTERMEDIATE_SCHOOL = details_habitations.HasIntermediateSchool == true ? "Y" : "N";
                        habitationDetails.MAST_DEGREE_COLLEGE = details_habitations.HasDegreeCollege == true ? "Y" : "N";
                        habitationDetails.MAST_HEALTH_SERVICE = details_habitations.HasHealthService == true ? "Y" : "N";
                        habitationDetails.MAST_DISPENSARY = details_habitations.HasDespensary == true ? "Y" : "N";
                        habitationDetails.MAST_PHCS = details_habitations.HasPHCS == true ? "Y" : "N";
                        habitationDetails.MAST_VETNARY_HOSPITAL = details_habitations.HasVetnaryHospital == true ? "Y" : "N";
                        habitationDetails.MAST_MCW_CENTERS = details_habitations.HasMCWCenters == true ? "Y" : "N";
                        habitationDetails.MAST_TELEGRAPH_OFFICE = details_habitations.HasTelegraphOffice == true ? "Y" : "N";
                        habitationDetails.MAST_TELEPHONE_CONNECTION = details_habitations.HasTelephoneConnection == true ? "Y" : "N";
                        habitationDetails.MAST_BUS_SERVICE = details_habitations.HasBusService == true ? "Y" : "N";
                        habitationDetails.MAST_RAILWAY_STATION = details_habitations.HasRailwayStation == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICTY = details_habitations.HasElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_TOURIST_PLACE = details_habitations.IsTouristPlace == true ? "Y" : "N";

                        //added by abhishek kamble 27-nov-2013
                        habitationDetails.USERID = PMGSYSession.Current.UserId;
                        habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(habitationDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        Models.MASTER_HABITATIONS habitationMaster = dbContext.MASTER_HABITATIONS.Where(hm => hm.MAST_HAB_CODE == habitationCode).FirstOrDefault();

                        if (habitationMaster != null && (habitationMaster.MAST_HAB_STATUS == "U" || habitationMaster.MAST_HAB_STATUS == "C"))
                        {
                            habitationMaster.MAST_HAB_STATUS = details_habitations.HasHabConnected == true ? "C" : "U";
                            //added by abhishek kamble 27-nov-2013
                            habitationMaster.USERID = PMGSYSession.Current.UserId;
                            habitationMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.Entry(habitationMaster).State = System.Data.Entity.EntityState.Modified;
                        }

                        dbContext.SaveChanges();
                        scope.Complete();
                    }

                    return true;
                }
                //added by ujjwal saket on19-10-2013 for PMGSY-II
                else
                {
                    int habitationCode = 0;
                    int year = 0;
                    encryptedParameters = details_habitations.EncryptedHabitationDetailsCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habitationCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());
                    year = Convert.ToInt32(decryptedParameters["Year"].ToString());

                    if (!CheckPopulation(false, dbContext, habitationCode, details_habitations, ref message))
                    {
                        return false;
                    }

                    Models.MASTER_HABITATIONS_DETAILS habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Where(habitation => habitation.MAST_HAB_CODE == habitationCode && habitation.MAST_YEAR == year).FirstOrDefault();

                    //check applied if change status unconnected to connected habitaion when edit

                    if (habitationDetails.MAST_HAB_CONNECTED.Equals("N") && details_habitations.HasHabConnected)
                    {
                        if (Convert.ToInt32(details_habitations.Years) != 2011) ///Changed by SAMMED A. PATIL on 13 APR 2017  for cgraipur issue as per directions from Srinivas Sir
                        {
                            int count = (from benefitedHabs in dbContext.IMS_BENEFITED_HABS
                                         join IMS in dbContext.IMS_SANCTIONED_PROJECTS
                                         on benefitedHabs.IMS_PR_ROAD_CODE equals IMS.IMS_PR_ROAD_CODE
                                         where
                                         benefitedHabs.MAST_HAB_CODE == habitationCode &&
                                         IMS.IMS_UPGRADE_CONNECT == "N"
                                         select IMS.IMS_PR_ROAD_CODE).Count();

                            if (count > 0)
                            {
                                message = "Habitation has been mapped to new type of proposal, so you can not update connectivity status to connected.";
                                return false;
                            }
                        }
                    }

                    if (dbContext.IMS_BENEFITED_HABS.Any(bh => bh.MAST_HAB_CODE == habitationCode))
                    {
                        var lstProposals = dbContext.IMS_BENEFITED_HABS.Where(m => m.MAST_HAB_CODE == habitationCode).Select(m => m.IMS_PR_ROAD_CODE);

                        if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => lstProposals.Contains(m.IMS_PR_ROAD_CODE) && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                        {
                            if (habitationDetails.MAST_HAB_TOT_POP != details_habitations.MAST_HAB_TOT_POP || habitationDetails.MAST_HAB_SCST_POP != details_habitations.MAST_HAB_SCST_POP)
                            {
                                message = "Habitation has been mapped to the proposal, so you can not update population.";
                                return false;
                            }
                        }
                    }


                    using (var scope = new TransactionScope())
                    {

                        habitationDetails.MAST_HAB_TOT_POP = details_habitations.MAST_HAB_TOT_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_TOT_POP;
                        habitationDetails.MAST_HAB_SCST_POP = details_habitations.MAST_HAB_SCST_POP == null ? 0 : (Int32)details_habitations.MAST_HAB_SCST_POP;
                        habitationDetails.MAST_HAB_CONNECTED = details_habitations.HasHabConnected == true ? "Y" : "N";
                        habitationDetails.MAST_PANCHAYAT_HQ = details_habitations.ISPanchayatHQ == true ? "Y" : "N";
                        // habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "Y" : "N";

                        //Modified By Abhishek kamble 29-Apr-2014
                        //habitationDetails.MAST_SCHEME = details_habitations.ISScheme == true ? "P" : "O";
                        habitationDetails.MAST_SCHEME = (details_habitations.HasHabConnected == false ? "N" : details_habitations.ISScheme == true ? "P" : "O");

                        habitationDetails.MAST_PRIMARY_SCHOOL = details_habitations.HasPrimarySchool == true ? "Y" : "N";
                        habitationDetails.MAST_MIDDLE_SCHOOL = details_habitations.HasMiddleSchool == true ? "Y" : "N";
                        habitationDetails.MAST_HIGH_SCHOOL = details_habitations.HasHighSchool == true ? "Y" : "N";
                        habitationDetails.MAST_INTERMEDIATE_SCHOOL = details_habitations.HasIntermediateSchool == true ? "Y" : "N";
                        habitationDetails.MAST_DEGREE_COLLEGE = details_habitations.HasDegreeCollege == true ? "Y" : "N";
                        habitationDetails.MAST_HEALTH_SERVICE = details_habitations.HasHealthService == true ? "Y" : "N";
                        habitationDetails.MAST_DISPENSARY = details_habitations.HasDespensary == true ? "Y" : "N";
                        habitationDetails.MAST_PHCS = details_habitations.HasPHCS == true ? "Y" : "N";
                        habitationDetails.MAST_VETNARY_HOSPITAL = details_habitations.HasVetnaryHospital == true ? "Y" : "N";
                        habitationDetails.MAST_MCW_CENTERS = details_habitations.HasMCWCenters == true ? "Y" : "N";
                        habitationDetails.MAST_TELEGRAPH_OFFICE = details_habitations.HasTelegraphOffice == true ? "Y" : "N";
                        habitationDetails.MAST_TELEPHONE_CONNECTION = details_habitations.HasTelephoneConnection == true ? "Y" : "N";
                        ///Added for Mining
                        habitationDetails.MAST_MINING = details_habitations.HasMining == true ? "Y" : "N";
                        habitationDetails.MAST_BUS_SERVICE = details_habitations.HasBusService == true ? "Y" : "N";
                        habitationDetails.MAST_RAILWAY_STATION = details_habitations.HasRailwayStation == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICTY = details_habitations.HasElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_TOURIST_PLACE = details_habitations.IsTouristPlace == true ? "Y" : "N";

                        habitationDetails.MAST_ITI = details_habitations.HasITI == true ? "Y" : "N";
                        habitationDetails.MAST_PETROL_PUMP = details_habitations.HasPetrolPump == true ? "Y" : "N";
                        habitationDetails.MAST_PUMP_ADD = details_habitations.HasAdditionalPetrolPump == true ? "Y" : "N";
                        habitationDetails.MAST_ELECTRICITY_ADD = details_habitations.HasAdditionalElectricity == true ? "Y" : "N";
                        habitationDetails.MAST_MANDI = details_habitations.HasMandi == true ? "Y" : "N";
                        habitationDetails.MAST_WAREHOUSE = details_habitations.HasWarehouse == true ? "Y" : "N";
                        habitationDetails.MAST_RETAIL_SHOP = details_habitations.HasRetailShop == true ? "Y" : "N";
                        habitationDetails.MAST_SUB_TEHSIL = details_habitations.HasSubTehsil == true ? "Y" : "N";
                        habitationDetails.MAST_BLOCK_HQ = details_habitations.HasBlockHeadquarter == true ? "Y" : "N";

                        //added by abhishek kamble 27-nov-2013
                        habitationDetails.USERID = PMGSYSession.Current.UserId;
                        habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(habitationDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        Models.MASTER_HABITATIONS habitationMaster = dbContext.MASTER_HABITATIONS.Where(hm => hm.MAST_HAB_CODE == habitationCode).FirstOrDefault();

                        if (habitationMaster != null && (habitationMaster.MAST_HAB_STATUS == "U" || habitationMaster.MAST_HAB_STATUS == "C"))
                        {
                            habitationMaster.MAST_HAB_STATUS = details_habitations.HasHabConnected == true ? "C" : "U";
                            //added by abhishek kamble 27-nov-2013
                            habitationMaster.USERID = PMGSYSession.Current.UserId;
                            habitationMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.Entry(habitationMaster).State = System.Data.Entity.EntityState.Modified;
                        }

                        dbContext.SaveChanges();
                        scope.Complete();
                    }

                    return true;
                }
                //finish addition

            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateOtherHabitationDetailsDAL().OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateOtherHabitationDetailsDAL().UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateOtherHabitationDetailsDAL()");
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



        #endregion OtherHabitationDetailsMasterDataEntry



        #region MLA-MP-Block Mapping

        public bool MapMLAConstituencyBlocksDAL(string encryptedMLAConstituencyCode, string encryptedBlockCodes)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                String[] blockCodes = null;
                int mlaConstituencyCode = 0;
                int blockCode = 0;
                Models.MASTER_MLA_BLOCKS master_mlaBlocks = null;
                encryptedParameters = encryptedMLAConstituencyCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstituencyCode"].ToString());

                //for all block codes
                blockCodes = encryptedBlockCodes.Split(',');

                if (blockCodes.Count() == 0)
                {
                    return false;
                }

                foreach (String item in blockCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                    //check record already exist with block code and mla constituency code if not then save else next                        
                    if (!(dbContext.MASTER_MLA_BLOCKS.Any(mla => mla.MAST_MLA_CONST_CODE == mlaConstituencyCode && mla.MAST_BLOCK_CODE == blockCode)))
                    {
                        master_mlaBlocks = new Models.MASTER_MLA_BLOCKS();

                        master_mlaBlocks.MAST_MLA_BLOCK_ID = (Int32)GetMaxCode(MasterDataEntryModules.MLABlocks);
                        master_mlaBlocks.MAST_MLA_CONST_CODE = mlaConstituencyCode;
                        master_mlaBlocks.MAST_BLOCK_CODE = blockCode;
                        master_mlaBlocks.MAST_MLA_BLOCK_ACTIVE = "Y";
                        master_mlaBlocks.MAST_LOCK_STATUS = "N";

                        //added by abhishek kamble 27-nov-2013
                        master_mlaBlocks.USERID = PMGSYSession.Current.UserId;
                        master_mlaBlocks.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MASTER_MLA_BLOCKS.Add(master_mlaBlocks);
                        dbContext.SaveChanges();
                    }
                    else///Changes by SAMMED A. PATIL to skip physical deletion of record
                    {
                        Models.MASTER_MLA_BLOCKS mlaBlocks = dbContext.MASTER_MLA_BLOCKS.Where(mla => mla.MAST_MLA_CONST_CODE == mlaConstituencyCode && mla.MAST_BLOCK_CODE == blockCode).FirstOrDefault();
                        mlaBlocks.MAST_MLA_BLOCK_ACTIVE = "Y";
                        mlaBlocks.USERID = PMGSYSession.Current.UserId;
                        mlaBlocks.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(mlaBlocks).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
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


        public bool MapMPConstituencyBlocksDAL(string encryptedMPConstituencyCode, string encryptedBlockCodes)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                String[] blockCodes = null;
                int mpConstituencyCode = 0;
                int blockCode = 0;
                Models.MASTER_MP_BLOCKS master_mpBlocks = null;
                encryptedParameters = encryptedMPConstituencyCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstituencyCode"].ToString());

                //for all block codes
                blockCodes = encryptedBlockCodes.Split(',');

                if (blockCodes.Count() == 0)
                {
                    return false;
                }

                foreach (String item in blockCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                    //check record already exist with block code and mla constituency code if not then save else next                        
                    if (!(dbContext.MASTER_MP_BLOCKS.Any(mp => mp.MAST_MP_CONST_CODE == mpConstituencyCode && mp.MAST_BLOCK_CODE == blockCode)))
                    {
                        master_mpBlocks = new Models.MASTER_MP_BLOCKS();

                        master_mpBlocks.MAST_MP_BLOCK_ID = (Int32)GetMaxCode(MasterDataEntryModules.MPBlocks);
                        master_mpBlocks.MAST_MP_CONST_CODE = mpConstituencyCode;
                        master_mpBlocks.MAST_BLOCK_CODE = blockCode;
                        master_mpBlocks.MAST_MP_BLOCK_ACTIVE = "Y";
                        master_mpBlocks.MAST_LOCK_STATUS = "N";

                        //added by abhishek kamble 27-nov-2013
                        master_mpBlocks.USERID = PMGSYSession.Current.UserId;
                        master_mpBlocks.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MASTER_MP_BLOCKS.Add(master_mpBlocks);
                        dbContext.SaveChanges();
                    }
                    else///Changes by SAMMED A. PATIL to skip physical deletion of record
                    {
                        Models.MASTER_MP_BLOCKS mpBlocks = dbContext.MASTER_MP_BLOCKS.Where(mla => mla.MAST_MP_CONST_CODE == mpConstituencyCode && mla.MAST_BLOCK_CODE == blockCode).FirstOrDefault();
                        mpBlocks.MAST_MP_BLOCK_ACTIVE = "Y";
                        mpBlocks.USERID = PMGSYSession.Current.UserId;
                        mpBlocks.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(mpBlocks).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
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

        #endregion MLA-MP-Block Mapping


        #region Panchayat-habitaions Mapping
        public bool MapPanchayatHabitationsDAL(string encryptedPanchayatCode, string encryptedHabCodes)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                String[] habCodes = null;
                int panchayatCode = 0;
                int habCode = 0;
                Models.MASTER_PANCHAYAT_HABITATIONS master_panHabs = null;
                encryptedParameters = encryptedPanchayatCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                panchayatCode = Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString());

                //for all habitation codes
                habCodes = encryptedHabCodes.Split(',');

                if (habCodes.Count() == 0)
                {
                    return false;
                }

                foreach (String item in habCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habCode = Convert.ToInt32(decryptedParameters["HabitationCode"].ToString());

                    //check record already exist with block code and mla constituency code if not then save else next                        
                    if (!(dbContext.MASTER_PANCHAYAT_HABITATIONS.Any(panHab => panHab.MAST_HAB_CODE == habCode)))
                    {
                        master_panHabs = new Models.MASTER_PANCHAYAT_HABITATIONS();

                        master_panHabs.MAST_PAN_HAB_CODE = (Int32)GetMaxCode(MasterDataEntryModules.PanchayatHabitation);
                        master_panHabs.MAST_PANCHAYAT_CODE = panchayatCode;
                        master_panHabs.MAST_HAB_CODE = habCode;
                        master_panHabs.MAST_PAN_HAB_ACTIVE = "Y";
                        master_panHabs.MAST_LOCK_STATUS = "N";

                        //added by abhishek kamble 27-nov-2013
                        master_panHabs.USERID = PMGSYSession.Current.UserId;
                        master_panHabs.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.MASTER_PANCHAYAT_HABITATIONS.Add(master_panHabs);
                        dbContext.SaveChanges();
                    }
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

        #endregion Panchayat-habitaions Mapping


        #region Shift District,Block,Village,Panchayat
        public bool ShiftDistrictDAL(string encryptedDistrictCodes, string newStateCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int districtCode = 0;
                int oldStateCode = 0;
                int stateCode = 0;

                encryptedParameters = encryptedDistrictCodes.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                districtCode = Convert.ToInt32(decryptedParameters["DistrictCode"].ToString());
                oldStateCode = Convert.ToInt32(decryptedParameters["StateCode"].ToString());
                stateCode = Convert.ToInt32(newStateCode);


                Int32 result = (from shiftDistrict in dbContext.sp_shift_district(oldStateCode, districtCode, stateCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftDistrict).FirstOrDefault();

                if (result == 1)
                {
                    return false;
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

        public bool ShiftBlockDAL(string encryptedBlockCode, string newDistictCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int districtCode = 0;
                int oldDistrictCode = 0;
                int blockCode = 0;

                encryptedParameters = encryptedBlockCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                oldDistrictCode = Convert.ToInt32(decryptedParameters["DistrictCode"].ToString());
                blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                districtCode = Convert.ToInt32(newDistictCode);

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftBlockDAL()");
                    sw.WriteLine("oldDistrictCode : " + oldDistrictCode.ToString());
                    sw.WriteLine("blockCode : " + blockCode.ToString());
                    sw.WriteLine("districtCode : " + districtCode.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 300;
                Int32 result = (from shiftBlock in dbContext.sp_shift_block(oldDistrictCode, blockCode, districtCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftBlock).FirstOrDefault();

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftBlockDAL()");
                    sw.WriteLine("result : " + result.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                if (result == 1)
                {
                    return false;
                }

                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftBlockDAL.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftBlockDAL.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftBlockDAL");
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

        public bool ShiftVillageDAL(string encryptedVillageCode, string newBlockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int blockCode = 0;
                int oldBlockCode = 0;
                int villageCode = 0;

                encryptedParameters = encryptedVillageCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                oldBlockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                blockCode = Convert.ToInt32(newBlockCode);

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillageDAL()");
                    sw.WriteLine("oldBlockCode : " + oldBlockCode.ToString());
                    sw.WriteLine("villageCode : " + villageCode.ToString());
                    sw.WriteLine("blockCode : " + blockCode.ToString());
                    sw.WriteLine("UserId : " + PMGSYSession.Current.UserId.ToString());
                    sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Int32 result = (from shiftVillage in dbContext.sp_shift_village(oldBlockCode, villageCode, blockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftVillage).FirstOrDefault();

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillageDAL()");
                    sw.WriteLine("result : " + result.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                if (result == 1)
                {
                    return false;
                }


                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftVillageDAL.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftVillageDAL.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "LocationMasterDataEntry.ShiftVillageDAL");
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

        public bool ShiftPanchayatDAL(string encryptedPanchayatCode, string newBlockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                int blockCode = 0;
                int oldBlockCode = 0;
                int panchayatCode = 0;

                encryptedParameters = encryptedPanchayatCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                oldBlockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
                panchayatCode = Convert.ToInt32(decryptedParameters["PanchayatCode"].ToString());
                blockCode = Convert.ToInt32(newBlockCode);

                Int32 result = (from shiftPanchayat in dbContext.sp_shift_panchayat(oldBlockCode, panchayatCode, blockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftPanchayat).FirstOrDefault();


                if (result == 1)
                {
                    return false;
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

        #endregion Shift District,Block,Village,Panchayat

        #region

        public bool DeleteDistrictDetailsDAL_ByDistrictCode(int districtCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_DISTRICT master_district = dbContext.MASTER_DISTRICT.Find(districtCode);

                if (master_district == null)
                {
                    return false;
                }


                //
                master_district.USERID = PMGSYSession.Current.UserId;
                master_district.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_district).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_DISTRICT.Remove(master_district);
                dbContext.SaveChanges();
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this District details.";
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



        public bool DeleteBlockDetailsDAL_ByBlockCode(int blockCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_BLOCK master_block = dbContext.MASTER_BLOCK.Find(blockCode);

                if (master_block == null)
                {
                    return false;
                }


                //added by abhishek kamble  26-nov-2013
                //master_block.USERID = PMGSYSession.Current.UserId;
                master_block.USERID = 27;
                master_block.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_block).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_BLOCK.Remove(master_block);
                dbContext.SaveChanges();
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this Block details.";
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

        public bool DeleteVillageDetailsDAL_ByVillageCode(int villageCode, ref string message)
        {
            Models.PMGSYEntities dbContextForMasterVillage = new Models.PMGSYEntities();
            Models.PMGSYEntities dbContextForVillagePopulation = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_VILLAGE master_village = dbContextForMasterVillage.MASTER_VILLAGE.Find(villageCode);

                Models.MASTER_VILLAGE_POPULATION master_village_population = dbContextForVillagePopulation.MASTER_VILLAGE_POPULATION.Where(m => m.MAST_VILLAGE_CODE == villageCode).FirstOrDefault();

                if (master_village == null)
                {
                    return false;
                }
                if (master_village_population == null)
                {
                    return false;
                }

                using (var scope = new TransactionScope())
                {
                    //added by abhishek kamble 26-nov-2013
                    master_village_population.USERID = PMGSYSession.Current.UserId;
                    master_village_population.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContextForVillagePopulation.Entry(master_village_population).State = System.Data.Entity.EntityState.Modified;
                    dbContextForVillagePopulation.SaveChanges();

                    //added on 22/10/2013 to remove records from population table
                    dbContextForVillagePopulation.Database.ExecuteSqlCommand("DELETE [omms].MASTER_VILLAGE_POPULATION Where MAST_VILLAGE_CODE = {0}", villageCode);
                    //dbContext.SaveChanges();

                    //added by abhishek kamble 26-nov-2013
                    master_village.USERID = PMGSYSession.Current.UserId;
                    master_village.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContextForMasterVillage.Entry(master_village).State = System.Data.Entity.EntityState.Modified;
                    dbContextForMasterVillage.SaveChanges();

                    dbContextForMasterVillage.MASTER_VILLAGE.Remove(master_village);
                    dbContextForMasterVillage.SaveChanges();
                    scope.Complete();

                }
                return true;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this Village details.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContextForMasterVillage != null)
                {
                    dbContextForMasterVillage.Dispose();
                }
                if (dbContextForVillagePopulation != null)
                {
                    dbContextForVillagePopulation.Dispose();
                }
            }
        }



        public bool DeletePanchayatDetailsDAL_ByPanchayatCode(int panchayatCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_PANCHAYAT master_panchayat = dbContext.MASTER_PANCHAYAT.Find(panchayatCode);

                if (master_panchayat == null)
                {
                    return false;
                }

                //added by abhishek kamble 26-nov-2013
                master_panchayat.USERID = PMGSYSession.Current.UserId;
                master_panchayat.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_panchayat).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_PANCHAYAT.Remove(master_panchayat);
                dbContext.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this Panchayat details.";
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



        public bool DeleteHabitationDetailsDAL_ByHabitationCode(int habitationCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            using (TransactionScope objScope = new TransactionScope())
            {
                try
                {
                    if (dbContext.PLAN_ROAD.Any(m => m.PLAN_RD_TO_HAB == habitationCode || m.PLAN_RD_FROM_HAB == habitationCode))
                    {
                        message = "Core Network starts or ends with this habitation. so it can not be deleted.";
                        return false;
                    }

                    Models.MASTER_HABITATIONS master_habitation = dbContext.MASTER_HABITATIONS.Find(habitationCode);

                    if (master_habitation == null)
                    {
                        return false;
                    }
                    List<int> PrimeIdList = null;
                    PrimeIdList = (from item in dbContext.MASTER_PANCHAYAT_HABITATIONS
                                   where item.MAST_HAB_CODE == habitationCode
                                   select item.MAST_PAN_HAB_CODE).ToList();
                    foreach (var PrimeId in PrimeIdList)
                    {

                        var master_panchayat_habitation = dbContext.MASTER_PANCHAYAT_HABITATIONS.Where(m => m.MAST_PAN_HAB_CODE == PrimeId).FirstOrDefault();
                        if (master_panchayat_habitation != null)
                        {
                            //added by abhishek kamble 27-nov-2013
                            master_panchayat_habitation.USERID = PMGSYSession.Current.UserId;
                            master_panchayat_habitation.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(master_panchayat_habitation).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.MASTER_PANCHAYAT_HABITATIONS.Remove(master_panchayat_habitation);
                            dbContext.SaveChanges();

                        }
                    }
                    PrimeIdList = (from item in dbContext.IMS_UNLOCK_DETAILS
                                   where item.MAST_HAB_CODE == habitationCode
                                   select item.IMS_TRANSACTION_NO).ToList();
                    foreach (var PrimeId in PrimeIdList)
                    {
                        var master_IMS_Unlock = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_TRANSACTION_NO == PrimeId).FirstOrDefault();
                        if (master_IMS_Unlock != null)
                        {
                            //added by abhishek kamble 27-nov-2013
                            master_IMS_Unlock.USERID = PMGSYSession.Current.UserId;
                            master_IMS_Unlock.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(master_IMS_Unlock).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.IMS_UNLOCK_DETAILS.Remove(master_IMS_Unlock);
                            dbContext.SaveChanges();

                        }
                    }
                    //PrimeIdList = (from item in dbContext.ADMIN_FEEDBACK
                    //               where item.MAST_HAB_CODE == habitationCode
                    //               select item.FEED_ID).ToList();
                    //   foreach (var PrimeId in PrimeIdList)
                    //   {
                    //       Models.ADMIN_FEEDBACK master_Admin_Feedback = dbContext.ADMIN_FEEDBACK.Where(m=>m.FEED_ID==PrimeId).FirstOrDefault();
                    //       if (master_Admin_Feedback != null)
                    //       {
                    //           //added by abhishek kamble 27-nov-2013


                    //           dbContext.ADMIN_FEEDBACK.Remove(master_Admin_Feedback);
                    //           dbContext.SaveChanges();

                    //       }
                    //   }

                    PrimeIdList = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                                   where item.MAST_HAB_CODE == habitationCode
                                   select item.MAST_ER_ROAD_ID).ToList();
                    foreach (var PrimeId in PrimeIdList)
                    {

                        var master_ER_Habi_Road = dbContext.MASTER_ER_HABITATION_ROAD.Where(a => a.MAST_ER_ROAD_ID == PrimeId).FirstOrDefault();
                        if (master_ER_Habi_Road != null)
                        {
                            //added by abhishek kamble 27-nov-2013
                            master_ER_Habi_Road.USERID = PMGSYSession.Current.UserId;
                            master_ER_Habi_Road.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(master_ER_Habi_Road).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.MASTER_ER_HABITATION_ROAD.Remove(master_ER_Habi_Road);
                            dbContext.SaveChanges();
                        }
                    }
                    PrimeIdList = (from item in dbContext.PLAN_ROAD_HABITATION
                                   where item.MAST_HAB_CODE == habitationCode
                                   select item.PLAN_CN_ROAD_HAB_ID).ToList();
                    foreach (var PrimeId in PrimeIdList)
                    {
                        var master_Plan_Road_Hab = dbContext.PLAN_ROAD_HABITATION.Where(a => a.PLAN_CN_ROAD_HAB_ID == PrimeId).FirstOrDefault();
                        if (master_Plan_Road_Hab != null)
                        {
                            //added by abhishek kamble 27-nov-2013
                            master_Plan_Road_Hab.USERID = PMGSYSession.Current.UserId;
                            master_Plan_Road_Hab.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(master_Plan_Road_Hab).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.PLAN_ROAD_HABITATION.Remove(master_Plan_Road_Hab);
                            dbContext.SaveChanges();

                        }
                    }
                    //added by abhishek kamble 27-nov-2013
                    master_habitation.USERID = PMGSYSession.Current.UserId;
                    master_habitation.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master_habitation).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_HABITATIONS.Remove(master_habitation);
                    dbContext.SaveChanges();


                    objScope.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    message = "You can not delete this Habitation details.";
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
        }


        public bool DeleteMLAConstituencyDetailsDAL_ByMLAConstituencyCode(int mlaConstituencyCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_MLA_CONSTITUENCY master_mlaConstituency = dbContext.MASTER_MLA_CONSTITUENCY.Find(mlaConstituencyCode);

                if (master_mlaConstituency == null)
                {
                    return false;
                }

                //Added by abhishek kamble 27-nov-2013
                master_mlaConstituency.USERID = PMGSYSession.Current.UserId;
                master_mlaConstituency.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_mlaConstituency).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_MLA_CONSTITUENCY.Remove(master_mlaConstituency);
                dbContext.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this MLA Constituency details.";
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



        public bool DeleteMPConstituencyDetailsDAL_ByMPConstituencyCode(int mpConstituencyCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                Models.MASTER_MP_CONSTITUENCY master_mpConstituency = dbContext.MASTER_MP_CONSTITUENCY.Find(mpConstituencyCode);

                if (master_mpConstituency == null)
                {
                    return false;
                }

                //Added by abhishek kamble 27-nov-2013
                master_mpConstituency.USERID = PMGSYSession.Current.UserId;
                master_mpConstituency.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master_mpConstituency).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_MP_CONSTITUENCY.Remove(master_mpConstituency);
                dbContext.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this MP Constituency details.";
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




        public bool DeleteHabitationOtherDetailsDAL_ByHabCodeandYear(int habitationCode, short year, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    Models.MASTER_HABITATIONS_DETAILS master_habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Find(habitationCode, year);

                    if (master_habitationDetails == null)
                    {
                        return false;
                    }

                    if (dbContext.IMS_BENEFITED_HABS.Any(bh => bh.MAST_HAB_CODE == habitationCode))
                    {
                        message = "You can not delete this habitation other details.";
                        return false;
                    }


                    //Added by abhishek kamble 27-nov-2013
                    master_habitationDetails.USERID = PMGSYSession.Current.UserId;
                    master_habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master_habitationDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();


                    dbContext.MASTER_HABITATIONS_DETAILS.Remove(master_habitationDetails);

                    Models.MASTER_HABITATIONS habitationMaster = dbContext.MASTER_HABITATIONS.Where(hm => hm.MAST_HAB_CODE == habitationCode).FirstOrDefault();


                    if (habitationMaster != null && habitationMaster.MAST_HAB_STATUS == "C")
                    {
                        habitationMaster.MAST_HAB_STATUS = "U";

                        //Added by abhishek kamble 27-nov-2013
                        habitationMaster.USERID = PMGSYSession.Current.UserId;
                        habitationMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(habitationMaster).State = System.Data.Entity.EntityState.Modified;
                    }

                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;

                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this habitation other details.";
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



        public Array GetMappedBlockDetailsListDAL_MLA(int mlaConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var query = from mlaConstituencyBlocks in dbContext.MASTER_MLA_BLOCKS
                            join blockDetails in dbContext.MASTER_BLOCK
                            on mlaConstituencyBlocks.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            where mlaConstituencyBlocks.MAST_MLA_BLOCK_ACTIVE == "Y" &&
                            blockDetails.MAST_BLOCK_ACTIVE == "Y" &&    //Added By Abhishek kamble 5-May-2014
                            districtDetails.MAST_DISTRICT_ACTIVE == "Y"//Added By Abhishek kamble 5-May-2014
                            &&
                            mlaConstituencyBlocks.MAST_MLA_CONST_CODE == mlaConstituencyCode
                            select new { blockDetails.MAST_BLOCK_CODE, blockDetails.MAST_BLOCK_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_IS_DESERT, blockDetails.MAST_IS_TRIBAL, blockDetails.MAST_PMGSY_INCLUDED, blockDetails.MAST_SCHEDULE5, mlaConstituencyBlocks.MAST_MLA_BLOCK_ID };

                totalRecords = query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "BlockName":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsDESERT":
                                query = query.OrderBy(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsTRIBAL":
                                query = query.OrderBy(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsPMGSYIncluded":
                                query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "BlockName":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsDESERT":
                                query = query.OrderByDescending(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsTRIBAL":
                                query = query.OrderByDescending(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsPMGSYIncluded":
                                query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }



                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(blockDetails => new
                {
                    blockDetails.MAST_BLOCK_CODE,
                    blockDetails.MAST_BLOCK_NAME,
                    blockDetails.MAST_DISTRICT_NAME,
                    blockDetails.MAST_IS_DESERT,
                    blockDetails.MAST_IS_TRIBAL,
                    blockDetails.MAST_PMGSY_INCLUDED,
                    blockDetails.MAST_SCHEDULE5,
                    blockDetails.MAST_MLA_BLOCK_ID
                    //blockDetails.MAST_DISTRICT_CODE

                }).ToArray();


                return result.Select(blockDetails => new
                {
                    cell = new[] {                         

                                blockDetails.MAST_BLOCK_NAME.ToString().Trim() ,                            
                                blockDetails.MAST_DISTRICT_NAME.ToString().Trim() ,  
                                blockDetails.MAST_IS_DESERT.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_IS_TRIBAL.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_PMGSY_INCLUDED.ToString().Trim()=="Y"?"Yes":"No",  
                                blockDetails.MAST_SCHEDULE5==null?"No":blockDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No",
                                "<a href='#' title='Click here to delete mapped block' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMappedBlock('" + URLEncrypt.EncryptParameters1(new string[]{"BlockId="+blockDetails.MAST_MLA_BLOCK_ID.ToString().Trim()}) +"'); return false;'>Delete Block</a>"
                                                    
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





        public Array GetMappedBlockDetailsListDAL_MP(int mpConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                var query = from mpConstituencyBlocks in dbContext.MASTER_MP_BLOCKS
                            join blockDetails in dbContext.MASTER_BLOCK
                            on mpConstituencyBlocks.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                            join districtDetails in dbContext.MASTER_DISTRICT
                            on blockDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                            where mpConstituencyBlocks.MAST_MP_BLOCK_ACTIVE == "Y" &&
                            blockDetails.MAST_BLOCK_ACTIVE == "Y" && //Added By Abhishek kamble 5-May-2014
                            districtDetails.MAST_DISTRICT_ACTIVE == "Y" &&//Added By Abhishek kamble 5-May-2014
                            mpConstituencyBlocks.MAST_MP_CONST_CODE == mpConstituencyCode
                            select new { blockDetails.MAST_BLOCK_CODE, blockDetails.MAST_BLOCK_NAME, districtDetails.MAST_DISTRICT_NAME, blockDetails.MAST_IS_DESERT, blockDetails.MAST_IS_TRIBAL, blockDetails.MAST_PMGSY_INCLUDED, blockDetails.MAST_SCHEDULE5, mpConstituencyBlocks.MAST_MP_BLOCK_ID };

                totalRecords = query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "BlockName":
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsDESERT":
                                query = query.OrderBy(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsTRIBAL":
                                query = query.OrderBy(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsPMGSYIncluded":
                                query = query.OrderBy(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "BlockName":
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "DistrictName":
                                query = query.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsDESERT":
                                query = query.OrderByDescending(x => x.MAST_IS_DESERT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsTRIBAL":
                                query = query.OrderByDescending(x => x.MAST_IS_TRIBAL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsPMGSYIncluded":
                                query = query.OrderByDescending(x => x.MAST_PMGSY_INCLUDED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }



                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(blockDetails => new
                {
                    blockDetails.MAST_BLOCK_CODE,
                    blockDetails.MAST_BLOCK_NAME,
                    blockDetails.MAST_DISTRICT_NAME,
                    blockDetails.MAST_IS_DESERT,
                    blockDetails.MAST_IS_TRIBAL,
                    blockDetails.MAST_PMGSY_INCLUDED,
                    blockDetails.MAST_SCHEDULE5,
                    blockDetails.MAST_MP_BLOCK_ID
                    //blockDetails.MAST_DISTRICT_CODE

                }).ToArray();


                return result.Select(blockDetails => new
                {
                    cell = new[] {                         

                                blockDetails.MAST_BLOCK_NAME.ToString().Trim() ,                            
                                blockDetails.MAST_DISTRICT_NAME.ToString().Trim() ,  
                                blockDetails.MAST_IS_DESERT.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_IS_TRIBAL.ToString().Trim()=="Y"?"Yes":"No",
                                blockDetails.MAST_PMGSY_INCLUDED.ToString().Trim()=="Y"?"Yes":"No",  
                                blockDetails.MAST_SCHEDULE5==null?"No":blockDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No",
                                "<a href='#' title='Click here to delete mapped district' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMappedBlock('" + URLEncrypt.EncryptParameters1(new string[]{"BlockId="+blockDetails.MAST_MP_BLOCK_ID.ToString().Trim()}) +"'); return false;'>Delete Block</a>"
                                                    
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



        public Array GetMappedHabitationDetailsListDAL_Panchayat(int panchayatCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {


                var query = from panchayatHab in dbContext.MASTER_PANCHAYAT_HABITATIONS
                            join habitationDetails in dbContext.MASTER_HABITATIONS
                            on panchayatHab.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                            join villageDetails in dbContext.MASTER_VILLAGE
                            on habitationDetails.MAST_VILLAGE_CODE equals villageDetails.MAST_VILLAGE_CODE
                            /*join mpContituency in dbContext.MASTER_MP_CONSTITUENCY
                            on habitationDetails.MAST_MP_CONST_CODE equals mpContituency.MAST_MP_CONST_CODE
                            join mlaContituency in dbContext.MASTER_MLA_CONSTITUENCY
                            on habitationDetails.MAST_MLA_CONST_CODE equals mlaContituency.MAST_MLA_CONST_CODE*/
                            where panchayatHab.MAST_PAN_HAB_ACTIVE == "Y" &&
                            panchayatHab.MAST_PANCHAYAT_CODE == panchayatCode
                            select new { habitationDetails.MAST_HAB_CODE, habitationDetails.MAST_HAB_NAME, villageDetails.MAST_VILLAGE_NAME, habitationDetails.MAST_SCHEDULE5, panchayatHab.MAST_PAN_HAB_CODE };

                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "HabitationName":
                                query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "VillageName":
                                query = query.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderBy(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "HabitationName":
                                query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "VillageName":
                                query = query.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "IsSchedule5":
                                query = query.OrderByDescending(x => x.MAST_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }


                //for testing

                //query = query.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

                var result = query.Select(habitationDetails => new
                {
                    habitationDetails.MAST_HAB_CODE,
                    habitationDetails.MAST_HAB_NAME,
                    habitationDetails.MAST_VILLAGE_NAME,
                    /* habitationDetails.MAST_MP_CONST_NAME,
                     habitationDetails.MAST_MLA_CONST_NAME,*/
                    habitationDetails.MAST_SCHEDULE5,
                    habitationDetails.MAST_PAN_HAB_CODE
                }).ToArray();


                return result.Select(habitationDetails => new
                {
                    cell = new[] {                         

                                habitationDetails.MAST_HAB_NAME.Trim() == string.Empty? "NA": habitationDetails.MAST_HAB_NAME.Trim(),                                   
                                habitationDetails.MAST_VILLAGE_NAME.ToString().Trim(),
                               /* habitationDetails.MAST_MP_CONST_NAME.ToString().Trim(),
                                habitationDetails.MAST_MLA_CONST_NAME.ToString().Trim(),*/
                                //habitationDetails.MAST_VILLAGE_TOT_POP == null ? "NA": habitationDetails.MAST_VILLAGE_TOT_POP.ToString(),                               
                                 habitationDetails.MAST_SCHEDULE5==null?"No":habitationDetails.MAST_SCHEDULE5.ToString().Trim()=="Y"?"Yes":"No", 
                                 "<a href='#' title='Click here to delete mapped habitation' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMappedHabitation('" + URLEncrypt.EncryptParameters1(new string[]{"HabCode="+habitationDetails.MAST_PAN_HAB_CODE.ToString().Trim()}) +"'); return false;'>Delete District</a>"
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

        public bool DeleteMappedHabitationDAL(int adminId)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Models.MASTER_PANCHAYAT_HABITATIONS habMaster = dbContext.MASTER_PANCHAYAT_HABITATIONS.Find(adminId);

                habMaster.USERID = PMGSYSession.Current.UserId;
                habMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(habMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.MASTER_PANCHAYAT_HABITATIONS.Remove(habMaster);
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


        public bool DeleteMappedMPBlockDAL(int blockId)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                Models.MASTER_MP_BLOCKS mpBlockMaster = dbContext.MASTER_MP_BLOCKS.Find(blockId);

                //Added by SAMMED A. PATIL 24 APRIL 2017 to change flag as inactive
                mpBlockMaster.MAST_MP_BLOCK_ACTIVE = "N";

                //added by abhishek kamble 27-nov-2013
                mpBlockMaster.USERID = PMGSYSession.Current.UserId;
                mpBlockMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(mpBlockMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                ///Commented by SAMMED A. PATIL on 24 APRIL 2017 to avoid physical deletion of records
                //dbContext.MASTER_MP_BLOCKS.Remove(mpBlockMaster);
                //dbContext.SaveChanges();
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

        public bool DeleteMappedMLABlockDAL(int blockId, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                //Check bock IsUsed in Habitation, Added By Abhishek kamble 25-Feb-2014 start

                int MlaConstCode = dbContext.MASTER_MLA_BLOCKS.Where(b => b.MAST_MLA_BLOCK_ID == blockId).Select(s => s.MAST_MLA_CONST_CODE).FirstOrDefault();

                //if (dbContext.MASTER_HABITATIONS.Any(m => m.MAST_MLA_CONST_CODE == MlaConstCode))
                //{
                //    message = "Mapped block details is in use and can not be deleted.";
                //    return false;
                //}

                //Check bock IsUsed in Habitation, Added By Abhishek kamble 25-Feb-2014 end



                Models.MASTER_MLA_BLOCKS mlaBlockMaster = dbContext.MASTER_MLA_BLOCKS.Find(blockId);

                //Added by SAMMED A. PATIL 24 APRIL 2017 to change flag as inactive
                mlaBlockMaster.MAST_MLA_BLOCK_ACTIVE = "N";

                //added by abhishek kamble 27-nov-2013
                mlaBlockMaster.USERID = PMGSYSession.Current.UserId;
                mlaBlockMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(mlaBlockMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                ///Commented by SAMMED A. PATIL on 24 APRIL 2017 to avoid physical deletion of records
                //dbContext.MASTER_MLA_BLOCKS.Remove(mlaBlockMaster);
                //dbContext.SaveChanges();
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


        public bool FinalizeStateDAL(int stateCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_STATE stateDetails = dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == stateCode).FirstOrDefault();
                stateDetails.MAST_LOCK_STATUS = "Y";
                stateDetails.USERID = PMGSYSession.Current.UserId;
                stateDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(stateDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeDistrictDAL(int districtCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_DISTRICT districtDetails = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();
                districtDetails.MAST_LOCK_STATUS = "Y";
                districtDetails.USERID = PMGSYSession.Current.UserId;
                districtDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(districtDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeBlockDAL(int blockCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_BLOCK blockDetails = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).FirstOrDefault();
                blockDetails.MAST_LOCK_STATUS = "Y";
                blockDetails.USERID = PMGSYSession.Current.UserId;
                blockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(blockDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool FinalizeVillageDAL(int villageCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {


                    int censusYear = PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011;
                    PMGSY.Models.MASTER_VILLAGE_POPULATION villageDetails = dbContext.MASTER_VILLAGE_POPULATION.Where(m => m.MAST_VILLAGE_CODE == villageCode && m.MAST_CENSUS_YEAR == censusYear).FirstOrDefault();
                    villageDetails.MAST_LOCK_STATUS = "Y";
                    villageDetails.USERID = PMGSYSession.Current.UserId;
                    villageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(villageDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 7-May-2014 start

                    PMGSY.Models.MASTER_VILLAGE masterVillageDetails = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == villageCode).FirstOrDefault();
                    masterVillageDetails.MAST_LOCK_STATUS = "Y";
                    villageDetails.USERID = PMGSYSession.Current.UserId;
                    villageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(masterVillageDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 7-May-2014 end  
                    ts.Complete();
                    return true;

                }
                catch (Exception)
                {
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool FinalizeHabitationDAL(int habitationCode, ref string message)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    Models.MASTER_HABITATIONS masterHabitation = null;

                    masterHabitation = dbContext.MASTER_HABITATIONS.Where(mh => mh.MAST_HAB_CODE == habitationCode).FirstOrDefault();

                    if (masterHabitation == null)
                    {
                        return false;
                    }

                    masterHabitation.MAST_LOCK_STATUS = "Y";
                    dbContext.Entry(masterHabitation).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 7-May-2014 start
                    int censusYear = PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011;
                    PMGSY.Models.MASTER_HABITATIONS_DETAILS habitationDetails = dbContext.MASTER_HABITATIONS_DETAILS.Where(m => m.MAST_HAB_CODE == habitationCode && m.MAST_YEAR == censusYear).FirstOrDefault();
                    habitationDetails.MAST_LOCK_STATUS = "Y";
                    habitationDetails.USERID = PMGSYSession.Current.UserId;
                    habitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(habitationDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    //Added By Abhishek kamble 7-May-2014 end
                    ts.Complete();
                    return true;

                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
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

        public bool FinalizePanchayatDAL(int panchayatCode, ref string message)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    List<int> PrimeIdList = null;
                    PrimeIdList = (from item in dbContext.MASTER_PANCHAYAT_HABITATIONS
                                   where item.MAST_PANCHAYAT_CODE == panchayatCode
                                   select item.MAST_PAN_HAB_CODE).ToList();
                    if (PrimeIdList.Count > 0)
                    {
                        foreach (var PrimeId in PrimeIdList)
                        {

                            var master_panchayat_habitation = dbContext.MASTER_PANCHAYAT_HABITATIONS.Where(m => m.MAST_PAN_HAB_CODE == PrimeId && m.MAST_PANCHAYAT_CODE == panchayatCode).FirstOrDefault();
                            if (master_panchayat_habitation != null)
                            {

                                master_panchayat_habitation.MAST_LOCK_STATUS = "Y";
                                master_panchayat_habitation.USERID = PMGSYSession.Current.UserId;
                                master_panchayat_habitation.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(master_panchayat_habitation).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        message = "This Panchayat against does not have any habitation details so that can not be finalize.";
                        return false;
                    }
                    //PMGSY.Models.MASTER_PANCHAYAT_HABITATIONS HabitationDetails = dbContext.MASTER_PANCHAYAT_HABITATIONS.Where(m => m.MAST_PANCHAYAT_CODE == panchayatCode).FirstOrDefault();
                    //if (HabitationDetails != null)
                    //{
                    //    HabitationDetails.MAST_LOCK_STATUS = "Y";
                    //    HabitationDetails.USERID = PMGSYSession.Current.UserId;
                    //    HabitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //    dbContext.Entry(HabitationDetails).State = System.Data.Entity.EntityState.Modified;
                    //    dbContext.SaveChanges();
                    //}
                    //else
                    //{
                    //    message = "This Panchayat against does not have any habitation details so that can not be finalize.";
                    //    return false;
                    //}

                    PMGSY.Models.MASTER_PANCHAYAT masterHabitationDetails = dbContext.MASTER_PANCHAYAT.Where(m => m.MAST_PANCHAYAT_CODE == panchayatCode).FirstOrDefault();
                    masterHabitationDetails.MAST_LOCK_STATUS = "Y";
                    masterHabitationDetails.USERID = PMGSYSession.Current.UserId;
                    masterHabitationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(masterHabitationDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble 7-May-2014 end  
                    ts.Complete();
                    return true;

                }
                catch (Exception)
                {
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool IsPMGSY3FinalizedDAL(int blockCode, int districtCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                if (!dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y")
                    && !dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Any(z => z.MAST_BLOCK_CODE == blockCode && z.IS_FINALIZED == "Y"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterDataEntryDAL.IsPMGSY3FinalizedDAL()");
                return false;
            }
        }

        #endregion


        #region Finalize Facility
        public string FinalizeFacilityDAL(int facilityID)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_FACILITY facilityModel = dbContext.MASTER_FACILITY.Find(facilityID);

                if (facilityModel.LATITUDE == null || facilityModel.LONGITUDE == null)
                {
                    return "Lattitude and Longitude are not available for this Facility. Details can not be finalized.";
                }

                if (facilityModel.FILE_NAME==null)
                {
                    return "Image not available for this Facility. Details can not be finalized.";
                }

                if (facilityModel != null)
                {
                    facilityModel.IS_FINALIZED = "Y";
                    facilityModel.FINALIZED_DATE = System.DateTime.Now;
                    facilityModel.USERID = PMGSYSession.Current.UserId;
                    facilityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(facilityModel).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();
                    return string.Empty;

                }
                else
                {
                    return "Invalid Facility Details";
                }
            }
            catch (Exception ex)
            {
                return "Error occurred while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public string DeFinalizeFacilityDAL(int facilityID)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_FACILITY facilityModel = dbContext.MASTER_FACILITY.Find(facilityID);
                if (facilityModel != null)
                {
                    facilityModel.IS_FINALIZED = "N";
                    facilityModel.FINALIZED_DATE = System.DateTime.Now;
                    facilityModel.USERID = PMGSYSession.Current.UserId;
                    facilityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(facilityModel).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();
                    return string.Empty;

                }
                else
                {
                    return "Invalid Facility Details";
                }
            }
            catch (Exception ex)
            {
                return "Error occurred while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public string DeFinalizeFacilityForAllDAL(List<int> facilityIDs) // Definalize all on itno login added by priyanka 10-08-2020
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                foreach (int id in facilityIDs)
                {
                    PMGSY.Models.MASTER_FACILITY facilityModel = dbContext.MASTER_FACILITY.Find(id);
                    if (facilityModel != null)
                    {
                        facilityModel.IS_FINALIZED = "N";
                        facilityModel.FINALIZED_DATE = System.DateTime.Now;
                        facilityModel.USERID = PMGSYSession.Current.UserId;
                        facilityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(facilityModel).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        return "Invalid Facility Details";
                    }
                }
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error occurred while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetFacilityDetailsListDALDefinalize(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                DateTime dateToCheck = new DateTime(2019, 08, 27);

                int facilitycode = Convert.ToInt32(ModelParam.ElementAt(4));
                int facilityType = Convert.ToInt32(ModelParam.ElementAt(5));
                int blockcode = Convert.ToInt32(ModelParam.ElementAt(7));
                int habitationcode = Convert.ToInt32(ModelParam.ElementAt(8));
                int distCode = Convert.ToInt32(ModelParam.ElementAt(6));

                var FacilityList = (from item in dbContext.MASTER_FACILITY

                                    join factID in dbContext.MASTER_FACILITY_CATEGORY on
                                    item.MASTER_FACILITY_SUB_CATEGORY_ID equals factID.MASTER_FACILITY_CATEGORT_ID

                                    join habcode in dbContext.FACILITY_HABITATION_MAPPING on
                                    item.MASTER_FACILITY_ID equals habcode.MASTER_FACILITY_ID

                                    join habname in dbContext.MASTER_HABITATIONS on
                                    habcode.MASTER_HAB_CODE equals habname.MAST_HAB_CODE

                                    join blockname in dbContext.MASTER_BLOCK on
                                    habcode.MASTER_BLOCK_CODE equals blockname.MAST_BLOCK_CODE

                                    join districtname in dbContext.MASTER_DISTRICT on
                                    habcode.MASTER_DISTRICT_CODE equals districtname.MAST_DISTRICT_CODE

                                    where
                                        //facilitycode == 0 ? true : factID.MASTER_FACILITY_PARENT_ID == facilitycode
                                    factID.MASTER_FACILITY_PARENT_ID == (facilitycode == 0 ? factID.MASTER_FACILITY_PARENT_ID : facilitycode)
                                    &&
                                        //facilityType == 0 ? true : item.MASTER_FACILITY_SUB_CATEGORY_ID == facilityType
                                    item.MASTER_FACILITY_SUB_CATEGORY_ID == (facilityType == 0 ? item.MASTER_FACILITY_SUB_CATEGORY_ID : facilityType)
                                    &&
                                        //blockcode == 0 ? true : blockname.MAST_BLOCK_CODE == blockcode
                                    blockname.MAST_BLOCK_CODE == (blockcode == 0 ? blockname.MAST_BLOCK_CODE : blockcode)
                                    &&
                                        //habitationcode == 0 ? true : habcode.MASTER_HAB_CODE == habitationcode
                                    habcode.MASTER_HAB_CODE == (habitationcode == 0 ? habcode.MASTER_HAB_CODE : habitationcode)
                                    &&
                                    habcode.MASTER_DISTRICT_CODE ==distCode // 419// PMGSYSession.Current.DistrictCode
                                    select new
                                    {
                                        item.MASTER_FACILITY_ID,
                                        factID.MASTER_FACILITY_CATEGORY_NAME,
                                        item.MASTER_FACILITY_DESC,
                                        item.MASTER_FACILITY_SUB_CATEGORY_ID,
                                        item.ADDRESS,
                                        item.MASTER_FACILITY_CATEGORY_ID,
                                        item.PINCODE,
                                        habcode.MASTER_HAB_CODE,
                                        habname.MAST_HAB_NAME,
                                        blockname.MAST_BLOCK_NAME,
                                        districtname.MAST_DISTRICT_NAME,
                                        districtname.MAST_DISTRICT_CODE,
                                        habcode.MASTER_BLOCK_CODE,
                                        factID.MASTER_FACILITY_CATEGORT_ID,
                                        item.FILE_NAME,
                                        item.FILE_UPLOAD_DATE,
                                        item.LATITUDE,
                                        item.LONGITUDE,
                                        item.IS_FINALIZED
                                    }).ToList();

                var habName = (from item in dbContext.FACILITY_HABITATION_MAPPING
                               join hab in dbContext.MASTER_HABITATIONS on item.MASTER_HAB_CODE equals hab.MAST_HAB_CODE

                               select new
                               {
                                   hab.MAST_HAB_CODE,
                                   hab.MAST_HAB_NAME
                               }).ToList();


                totalRecords = FacilityList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "FacilityID":
                                FacilityList = FacilityList.OrderBy(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "FacilityID":
                                FacilityList = FacilityList.OrderByDescending(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    FacilityList = FacilityList.OrderBy(x => x.MASTER_FACILITY_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var facility = FacilityList.Select(facilityDetails => new
                {
                    facilityDetails.MASTER_FACILITY_ID,
                    facilityDetails.MASTER_FACILITY_CATEGORY_ID,
                    facilityDetails.MAST_HAB_NAME,
                    facilityDetails.MASTER_FACILITY_DESC,
                    facilityDetails.MASTER_FACILITY_CATEGORY_NAME,
                    facilityDetails.ADDRESS,
                    facilityDetails.PINCODE,
                    facilityDetails.MAST_DISTRICT_NAME,
                    facilityDetails.MAST_BLOCK_NAME,
                    facilityDetails.FILE_NAME,
                    facilityDetails.FILE_UPLOAD_DATE,
                    facilityDetails.LATITUDE,
                    facilityDetails.LONGITUDE,
                    facilityDetails.IS_FINALIZED
                }).ToArray();

                return facility.Select(fac => new
                {
                    cell = new[]
                {
                 
                    URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +  fac.MASTER_FACILITY_ID.ToString()}),
                    fac.MASTER_FACILITY_CATEGORY_ID.ToString(),
                    fac.MAST_DISTRICT_NAME,
                    fac.MAST_BLOCK_NAME,
                    fac.MAST_HAB_NAME.ToString(),
                    GetFacilityCategory(fac.MASTER_FACILITY_CATEGORY_ID),
                    fac.MASTER_FACILITY_CATEGORY_NAME.ToString(),
                    fac.MASTER_FACILITY_DESC.ToString(),
                    fac.ADDRESS.ToString(),
                    fac.PINCODE.ToString(),
                    fac.FILE_NAME != null ? Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD_VIRTUAL_DIR_PATH"] , fac.FILE_NAME) + "#" + 
                    (fac.FILE_NAME == null  ? " " :
                    
                    (fac.IS_FINALIZED=="Y"  || fac.FILE_UPLOAD_DATE > dateToCheck
                    
                    && (File.Exists(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] , fac.FILE_NAME))?
                       (Nullable<long>) new FileInfo(Path.Combine(ConfigurationManager.AppSettings["FACILITY_FILE_UPLOAD"] , fac.FILE_NAME)).Length != 0 : false)

                    ? " " : 
                    
                    " ")) : " ",

                    (fac.LATITUDE==null||fac.LONGITUDE==null||fac.FILE_NAME==null)
                   
                   ? "No": (fac.IS_FINALIZED == null ? "No" : (fac.IS_FINALIZED.Equals("Y") ? "Yes" : "No")),


                   //(fac.LATITUDE==null||fac.LONGITUDE==null||fac.FILE_NAME==null)
                   
                   //? "-": (fac.IS_FINALIZED == null ? "No" : (fac.IS_FINALIZED.Equals("Y") ? "Yes" : "No")),
 
                   dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockcode).FirstOrDefault() != null ? 
                   (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockcode).FirstOrDefault().IS_FINALIZED.Equals("Y")? true.ToString() : false.ToString()) : false.ToString(),
                    
                    fac.FILE_NAME == null && (fac.LATITUDE != null || fac.LONGITUDE != null)?
                    "<center><span class='ui-icon ui-icon-image' title='Click here to upload photograph' onClick ='UploadFacilityPhotoGraph(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +    fac.MASTER_FACILITY_ID.ToString()}) + 
                    "\");'></span></center>"
                    : "-",

                 //   fac.IS_FINALIZED==null?"-":(fac.IS_FINALIZED=="Y"?"<center><span class='ui-icon ui-icon-locked' title='Click here to definalize facility' onClick ='definalizeFacility(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +fac.MASTER_FACILITY_ID.ToString()})+ "\");'></span></center>":"<center><span class='ui-icon ui-icon-unlocked' title='Facility is definalized' onClick ='noAction(\"" + URLEncrypt.EncryptParameters1(new string[] { "FacilityCode =" +fac.MASTER_FACILITY_ID.ToString()})+ "\");'></span></center>")

                    //"<input type='button' value='Delete Image' class='jqueryButton' title='click here to delete image' onClick = DeleteImageFromGrid('" + fac.MASTER_FACILITY_ID + "') />")) : " ",
                    
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "MasterDataEntryDAl.GetFacilityDetailsListDAL()");
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


        #region Shift Village to New Block 11 JAN 2021 
         
        public bool ShiftVillageToNewBlock(string encryptedVillageCode, string newBlockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int blockCode = 0;
                int oldBlockCode = 0;
                int villageCode = 0;

                encryptedParameters = encryptedVillageCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                oldBlockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                blockCode = Convert.ToInt32(newBlockCode); // new Block Code.

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftVillageToNewBlock()");
                    sw.WriteLine("oldBlockCode : " + oldBlockCode.ToString());
                    sw.WriteLine("villageCode : " + villageCode.ToString());
                    sw.WriteLine("blockCode : " + blockCode.ToString());
                    sw.WriteLine("UserId : " + PMGSYSession.Current.UserId.ToString());
                    sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

               // Int32 result = (from shiftVillage in dbContext.sp_shift_village(oldBlockCode, villageCode, blockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftVillage).FirstOrDefault();

                PMGSY.Models.MASTER_VILLAGE masterVillage = new Models.MASTER_VILLAGE();

                masterVillage = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == villageCode).FirstOrDefault();

                if (masterVillage != null)
                {
                    masterVillage.MAST_BLOCK_CODE = blockCode; // New Block Code is assigned here.
                    dbContext.Entry(masterVillage).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
                


                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "MasterdataEntryDAL.ShiftVillageToNewBlock()");
                //  //  sw.WriteLine("result : " + result.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                //if (result == 1)
                //{
                //    return false;
                //}


                //return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftVillageToNewBlock.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftVillageToNewBlock.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftVillageToNewBlock");
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

        public bool ShiftHabToNewVillageDAL(string encryptedHabCode, string newBlockCode, string newVillageCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int blockCode = 0;
                int oldBlockCode = 0;
                int habCode = 0;
                int newVillageCODE=0;

                encryptedParameters = encryptedHabCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
               // oldBlockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());

                habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString()); // Hab Code to be shifted to New Village
                blockCode = Convert.ToInt32(newBlockCode); // new Block Code.
                newVillageCODE=Convert.ToInt32(newVillageCode);  // new Village Code.

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftHabToNewVillageDAL()");
                    sw.WriteLine("oldBlockCode : " + oldBlockCode.ToString());
                    sw.WriteLine("habCode : " + habCode.ToString());
                    sw.WriteLine("blockCode : " + blockCode.ToString());
                    sw.WriteLine("newVillageCODE : " + newVillageCODE.ToString());

                    sw.WriteLine("UserId : " + PMGSYSession.Current.UserId.ToString());
                    sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                // Int32 result = (from shiftVillage in dbContext.sp_shift_village(oldBlockCode, villageCode, blockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]) select (Int32)shiftVillage).FirstOrDefault();

                PMGSY.Models.MASTER_HABITATIONS masterHab = new Models.MASTER_HABITATIONS();

                masterHab = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == habCode).FirstOrDefault();

                if (masterHab != null)
                {
                    masterHab.MAST_VILLAGE_CODE = newVillageCODE; // New Block Code is assigned here.
                    dbContext.Entry(masterHab).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }



                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "MasterdataEntryDAL.ShiftVillageToNewBlock()");
                //  //  sw.WriteLine("result : " + result.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                //if (result == 1)
                //{
                //    return false;
                //}


                //return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftHabToNewVillageDAL.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftHabToNewVillageDAL.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MasterdataEntryDAL.ShiftHabToNewVillageDAL.Exception");
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


        //public List<Models.MASTER_VILLAGE> GetAllVillagesByBlockCode(int districtCode, bool isSearch)
        //{
        //    Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
        //    try
        //    {
        //        List<Models.MASTER_BLOCK> blockList = null;

        //        if (PMGSYSession.Current.RoleCode == 23)
        //        {
        //            blockList = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode).OrderBy(b => b.MAST_BLOCK_NAME).ToList<Models.MASTER_BLOCK>();
        //        }
        //        else
        //        {
        //            blockList = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode && b.MAST_BLOCK_ACTIVE == "Y").OrderBy(b => b.MAST_BLOCK_NAME).ToList<Models.MASTER_BLOCK>();
        //        }

        //        if (isSearch)
        //        {
        //            blockList.Insert(0, new Models.MASTER_BLOCK() { MAST_BLOCK_CODE = 0, MAST_BLOCK_NAME = "All Blocks" });
        //        }
        //        else
        //        {
        //            blockList.Insert(0, new Models.MASTER_BLOCK() { MAST_BLOCK_CODE = 0, MAST_BLOCK_NAME = "--Select--" });
        //        }

        //        return blockList;

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        #endregion


    } //end MasterDataEntryDAL:IMasterDataEntryDAL


    public interface IMasterDataEntryDAL
    {
        #region Addition of Facitlity
        Array GetFacilityDetailsListDALDefinalize(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam);


        string GetFacilityCategory(Int32 MASTER_FACILITY_CATEGORY_ID);

        bool DeleteFacilityDAL(int id);

        Array GetFacilityDetailsListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam);

        bool SaveFacilityDetailsDAL(CreateFacility createFacility, ref string message);

        CreateFacility DisplayfacilityDetailsDAL(int FacilityID);

        bool DeleteImageLatLongDAL(int facilityID, string remark);

        bool SavePhotoGraphDAL(int facilityID, string filename, HttpPostedFileBase filebase);
        #endregion


        #region

        bool SaveStateDetailsDAL(MASTER_STATE master_state, ref string message);


        MASTER_STATE GetStateDetailsDAL_ByStateCode(int stateCode);


        Array GetStateDetailsListDAL(bool isMap, int? page, int? rows, string sidx, string sord, out long totalRecords, int stateUT, int stateType);

        bool UpdateStateDetailsDAL(MASTER_STATE master_state, ref string message);

        bool DeleteStateDetailsDAL_ByStateCode(int stateCode, ref string message);

        bool SaveDistrictDetailsDAL(MASTER_DISTRICT master_district, ref string message);

        Array GetDistrictDetailsListDAL(int agencyCode, int regionCode, int adminNdCode, bool isMap, MappingType mapping, int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        MASTER_DISTRICT GetDistrictDetailsDAL_ByDistrictCode(int districtCode);

        bool UpdateDistrictDetailsDAL(MASTER_DISTRICT master_district, ref string message);

        Array GetBlockDetailsListDAL(bool isMap, bool isMLA, int stateCode, int districtCode, int MLAConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool SaveBlockDetailsDAL(BlockMaster master_block, ref string message);

        BlockMaster GetBlockDetailsDAL_ByBlockCode(int blockCode);

        bool UpdateBlockDetailsDAL(BlockMaster master_block, ref string message);

        Array GetVillageDetailsListDAL(int stateCode, int districtCode, int blockCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool SaveVillageDetailsDAL(VillageMaster master_village, ref string message);

        VillageMaster GetVillageDetailsDAL_ByVillageCode(int villageCode);

        bool UpdateVillageDetailsDAL(VillageMaster master_village, ref string message);

        Array GetHabitationDetailsListDAL(bool isMap, int stateCode, int districtCode, int blockCode, string villageName, string habitationName, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveHabitationDetailsDAL(HabitationMaster master_habitations, ref string message);

        HabitationMaster GetHabitationDetailsDAL_ByHabitationCode(int habitationCode);

        bool UpdateHabitationDetailsDAL(HabitationMaster master_habitations, ref string message);

        Array GetPanchayatDetailsListDAL(int stateCode, int districtCode, int blockCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool SavePanchayatDetailsDAL(PanchayatMaster master_panchayat, ref string message);

        PanchayatMaster GetPanchayatDetailsDAL_ByPanchayatCode(int panchayatCode);

        bool UpdatePanchayatDetailsDAL(PanchayatMaster master_panchayat, ref string message);

        Array GetMLAConstituencyDetailsListDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool SaveMLAConstituencyDetailsDAL(MLAConstituency master_mlaconstituency, ref string message);

        MLAConstituency GetMLAConstituencyDetailsDAL_ByMLAConstituencyCode(int mlaConstituencyCode);

        bool UpdateMLAConstituencyDetailsDAL(MLAConstituency master_mlaconstituency, ref string message);

        Array GetMPConstituencyDetailsListDAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveMPConstituencyDetailsDAL(MPConstituency master_mpconstituency, ref string message);

        MPConstituency GetMPConstituencyDetailsDAL_ByMPConstituencyCode(int mpConstituencyCode);

        bool UpdateMPConstituencyDetailsDAL(MPConstituency master_mpconstituency, ref string message);

        Array GetOtherHabitationDetailsListDAL(int habitationCode, string lockStatus, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool SaveOtherHabitationDetailsDAL(HabitationDetails details_habitations, ref string message);

        HabitationDetails GetOtherHabitationDetailsDAL_ByHabitationCodeandYear(int habitationCode, short year);

        bool UpdateOtherHabitationDetailsDAL(HabitationDetails details_habitations, ref string message);

        //Added By Abhishek kamble 24-feb-2014        
        bool CheckRemainingPopulation(bool isAdd, Models.PMGSYEntities dbContext, int habitationCode, ref int totalRemainingPopulation, ref int totalRemainingSCSTPopulation, ref int totalVillagePopulation, ref Int64 totalVillagePopulation20Per, ref int totalVillageSCSTPopulation, ref string message);

        bool MapMLAConstituencyBlocksDAL(string encryptedMLAConstituencyCode, string encryptedBlockCodes);

        bool MapMPConstituencyBlocksDAL(string encryptedMPConstituencyCode, string encryptedBlockCodes);

        bool MapPanchayatHabitationsDAL(string encryptedPanchayatCode, string encryptedHabCodes);

        bool ShiftDistrictDAL(string encryptedDistrictCodes, string newStateCode);

        bool ShiftBlockDAL(string encryptedBlockCode, string newDistictCode);

        bool ShiftVillageDAL(string encryptedVillageCode, string newBlockCode);

        bool ShiftPanchayatDAL(string encryptedPanchayatCode, string newBlockCode);

        bool DeleteDistrictDetailsDAL_ByDistrictCode(int districtCode, ref string message);

        bool DeleteBlockDetailsDAL_ByBlockCode(int blockCode, ref string message);

        bool DeleteVillageDetailsDAL_ByVillageCode(int villageCode, ref string message);

        bool DeletePanchayatDetailsDAL_ByPanchayatCode(int panchayatCode, ref string message);

        bool DeleteHabitationDetailsDAL_ByHabitationCode(int habitationCode, ref string message);

        bool DeleteMLAConstituencyDetailsDAL_ByMLAConstituencyCode(int mlaConstituencyCode, ref string message);

        bool DeleteMPConstituencyDetailsDAL_ByMPConstituencyCode(int mpConstituencyCode, ref string message);

        bool DeleteHabitationOtherDetailsDAL_ByHabCodeandYear(int habitationCode, short year, ref string message);

        Array GetMappedBlockDetailsListDAL_MLA(int mlaConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        Array GetMappedBlockDetailsListDAL_MP(int mpConstituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        Array GetMappedHabitationDetailsListDAL_Panchayat(int panchayatCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool FinalizeHabitationDAL(int habitationCode, ref string message);

        bool DeleteMappedHabitationDAL(int adminId);

        bool DeleteMappedMPBlockDAL(int blockId);

        bool DeleteMappedMLABlockDAL(int blockId, ref string message);


        bool FinalizeStateDAL(int stateCode);

        bool FinalizeDistrictDAL(int districtCode);

        bool FinalizeBlockDAL(int blockCode);

        bool FinalizeVillageDAL(int villageCode);
        bool FinalizePanchayatDAL(int panchayatCode, ref string message);


        #endregion


        #region Shift Village to New Block 11 JAN 2021 
         
         bool ShiftVillageToNewBlock(string encryptedVillageCode, string newBlockCode);
         bool ShiftHabToNewVillageDAL(string encryptedHabCode, string newBlockCode, string newVillageCode);
        #endregion
    }

}