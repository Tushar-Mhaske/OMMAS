using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;



namespace  PMGSY.DAL.Common
{
    public class GeneralFunctionsDAL : IGeneralFunctionsDAL
    {


        public GeneralFunctionsDAL()
        {
           
        }

        //#region Function GetFinancial Year
        //public List<FinancialYearDTO> GetFinancialYear(char level, int StateCode, int? DistrictCode, int? BlockCode, long? GramPanchayatCode)
        //{
        //    FinancialYearDTO objFinancialYearDTO = new FinancialYearDTO();
        //    List<FinancialYearDTO> objListFinancialYearDTO = new List<FinancialYearDTO>();
        //    NpgsqlDataReader rdFinancialYearDetails = null;
        //    NpgsqlConnection conn = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //    NpgsqlCommand command = new NpgsqlCommand();
        //    NpgsqlParameter objParameter = new NpgsqlParameter();

        //    conn.Open();
        //    NpgsqlTransaction objTransaction = conn.BeginTransaction();
        //    try
        //    {

        //        command.Connection = conn;
        //        command.CommandType = System.Data.CommandType.StoredProcedure;

        //        command.CommandText = "nlma.uspacclistfinancialyear";

        //        objParameter = new NpgsqlParameter("charlevel", level);
        //        objParameter.Direction = ParameterDirection.Input;
        //        objParameter.NpgsqlDbType = NpgsqlDbType.Varchar;
        //        command.Parameters.Add(objParameter);

        //        objParameter = new NpgsqlParameter("statecode", StateCode);
        //        objParameter.Direction = System.Data.ParameterDirection.Input;
        //        objParameter.NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters.Add(objParameter);

        //        objParameter = new NpgsqlParameter("districtcode", DistrictCode);
        //        objParameter.Direction = System.Data.ParameterDirection.Input;
        //        objParameter.NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters.Add(objParameter);

        //        objParameter = new NpgsqlParameter("blockcode", BlockCode);
        //        objParameter.Direction = System.Data.ParameterDirection.Input;
        //        objParameter.NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters.Add(objParameter);

        //        objParameter = new NpgsqlParameter("grampanchayatcode", GramPanchayatCode);
        //        objParameter.Direction = System.Data.ParameterDirection.Input;
        //        objParameter.NpgsqlDbType = NpgsqlDbType.Bigint;
        //        command.Parameters.Add(objParameter);

        //        objParameter = new NpgsqlParameter("refyear", NpgsqlDbType.Refcursor);
        //        objParameter.Direction = System.Data.ParameterDirection.InputOutput;
        //        command.Parameters.Add(objParameter);

        //        rdFinancialYearDetails = command.ExecuteReader();

        //        while (rdFinancialYearDetails.Read())
        //        {
        //            objFinancialYearDTO.OBMonth = rdFinancialYearDetails[0].ToString();
        //            objFinancialYearDTO.OBYear = rdFinancialYearDetails.GetValue(1).ToString();
        //            objListFinancialYearDTO.Add(objFinancialYearDTO);
        //        }

        //        // objTransaction.Commit();
        //        // conn.Close();
        //        return objListFinancialYearDTO;
        //    }
        //    catch (NpgsqlException Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objGetFinancialYearException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DGetFinancialYear, "Error While Getting Financial Year....", Ex);
        //        throw objGetFinancialYearException;
        //    }

        //    finally
        //    {
        //        objTransaction.Commit();
        //        conn.Close();
        //    }

        //}
        //#endregion

        //#region Function Pupulate State
        //public List<StateMasterDTO> PopulateState()
        //{
        //    NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //    conPGSqlConnection.Open();
        //    NpgsqlTransaction trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //    try
        //    {
        //        #region Variable Declaration
        //        //NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //        NpgsqlCommand cmdPGSqlCommand = new NpgsqlCommand();
        //        DataSet dsPGSqlDataSet = new DataSet();
        //        NpgsqlDataAdapter adptPGSqlDataAdptr;
        //        // NpgsqlTransaction trnPGSqlTransaction;
        //        List<StateMasterDTO> objListStateMasterDTO = new List<StateMasterDTO>();
        //        int intListCount = 0;
        //        #endregion
        //        #region Set Connection and Open it and Execute Stored Procedure
        //        // conPGSqlConnection.Open();
        //        // trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //        cmdPGSqlCommand.CommandType = CommandType.StoredProcedure;
        //        cmdPGSqlCommand.Connection = conPGSqlConnection;
        //        //cmdPGSqlCommand.CommandText = "nlma.usp_acc_populatestate";
        //        cmdPGSqlCommand.CommandText = "nlma.ufn_gf_populatestate";
        //        adptPGSqlDataAdptr = new NpgsqlDataAdapter(cmdPGSqlCommand);
        //        adptPGSqlDataAdptr.Fill(dsPGSqlDataSet);
        //        //trnPGSqlTransaction.Commit();
        //        // conPGSqlConnection.Close();
        //        #endregion
        //        #region Fill List
        //        intListCount = dsPGSqlDataSet.Tables[0].Rows.Count;
        //        for (int i = 0; i < intListCount; i++)
        //        {
        //            StateMasterDTO objStateMasterDTO = new StateMasterDTO();
        //            objStateMasterDTO.StateCode = dsPGSqlDataSet.Tables[0].Rows[i][0].ToString();
        //            objStateMasterDTO.StateName = dsPGSqlDataSet.Tables[0].Rows[i][1].ToString();
        //            objListStateMasterDTO.Add(objStateMasterDTO);
        //        }
        //        #endregion
        //        return objListStateMasterDTO;
        //    }
        //    catch (NpgsqlException Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objPopulateStateException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DPopulateState, "Error While Populating the State....", Ex);
        //        throw objPopulateStateException;
        //    }

        //    finally
        //    {
        //        trnPGSqlTransaction.Commit();
        //        conPGSqlConnection.Close();
        //    }

        //}
        //#endregion

        //#region Function Populate District
        //public List<DistrictMasterDTO> PopulateDistrict(Int32 StateCode)
        //{
        //    NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //    conPGSqlConnection.Open();
        //    NpgsqlTransaction trnPGSqlTransaction = trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //    try
        //    {
        //        #region Variable Declaration
        //        // NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //        NpgsqlCommand cmdPGSqlCommand = new NpgsqlCommand();
        //        DataSet dsPGSqlDataSet = new DataSet();
        //        NpgsqlDataAdapter adptPGSqlDataAdptr;
        //        // NpgsqlTransaction trnPGSqlTransaction;
        //        List<DistrictMasterDTO> objListDistrictMasterDTO = new List<DistrictMasterDTO>();
        //        int intListCount = 0;
        //        #endregion
        //        #region Set Connection and Open it and Execute Stored Procedure
        //        //conPGSqlConnection.Open();
        //        // trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //        cmdPGSqlCommand.CommandType = CommandType.StoredProcedure;
        //        cmdPGSqlCommand.Connection = conPGSqlConnection;
        //        cmdPGSqlCommand.CommandText = "nlma.ufn_gf_populatedistrict";

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
        //        cmdPGSqlCommand.Parameters[0].Value = StateCode;

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[1].NpgsqlDbType = NpgsqlDbType.Refcursor;


        //        adptPGSqlDataAdptr = new NpgsqlDataAdapter(cmdPGSqlCommand);
        //        adptPGSqlDataAdptr.Fill(dsPGSqlDataSet);
        //        // trnPGSqlTransaction.Commit();
        //        // conPGSqlConnection.Close();  
        //        #endregion
        //        #region Fill List
        //        intListCount = dsPGSqlDataSet.Tables[0].Rows.Count;
        //        for (int i = 0; i < intListCount; i++)
        //        {
        //            DistrictMasterDTO objDistrictMasterDTO = new DistrictMasterDTO();
        //            objDistrictMasterDTO.DistrictCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][0]);
        //            objDistrictMasterDTO.DistrictName = dsPGSqlDataSet.Tables[0].Rows[i][1].ToString();
        //            objDistrictMasterDTO.StateCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][2]);
        //            objListDistrictMasterDTO.Add(objDistrictMasterDTO);
        //        }
        //        #endregion
        //        return objListDistrictMasterDTO;
        //    }
        //    catch (NpgsqlException Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objPopulateDistrictException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DPopulateDistrict, "Error While Populating the District....", Ex);
        //        throw objPopulateDistrictException;
        //    }

        //    finally
        //    {
        //        trnPGSqlTransaction.Commit();
        //        conPGSqlConnection.Close();
        //    }

        //}
        //#endregion

        //#region Function Populate Block
        //public List<MasterBlockDTO> PopulateBlock(Int32 DistrictCode)
        //{
        //    NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //    conPGSqlConnection.Open();
        //    NpgsqlTransaction trnPGSqlTransaction = trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //    try
        //    {

        //        #region Variable Declaration
        //        // NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //        NpgsqlCommand cmdPGSqlCommand = new NpgsqlCommand();
        //        DataSet dsPGSqlDataSet = new DataSet();
        //        NpgsqlDataAdapter adptPGSqlDataAdptr;
        //        // NpgsqlTransaction trnPGSqlTransaction;
        //        List<MasterBlockDTO> objListBlockMasterDTO = new List<MasterBlockDTO>();
        //        int intListCount = 0;
        //        #endregion
        //        #region Set Connection and Open it and Execute Stored Procedure
        //        //  conPGSqlConnection.Open();
        //        // trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //        cmdPGSqlCommand.CommandType = CommandType.StoredProcedure;
        //        cmdPGSqlCommand.Connection = conPGSqlConnection;
        //        cmdPGSqlCommand.CommandText = "nlma.ufn_gf_populateblock";

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
        //        cmdPGSqlCommand.Parameters[0].Value = DistrictCode;

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[1].NpgsqlDbType = NpgsqlDbType.Refcursor;


        //        adptPGSqlDataAdptr = new NpgsqlDataAdapter(cmdPGSqlCommand);
        //        adptPGSqlDataAdptr.Fill(dsPGSqlDataSet);
        //        // trnPGSqlTransaction.Commit();
        //        // conPGSqlConnection.Close();
        //        #endregion
        //        #region Fill List
        //        intListCount = dsPGSqlDataSet.Tables[0].Rows.Count;
        //        for (int i = 0; i < intListCount; i++)
        //        {
        //            MasterBlockDTO objBlockMasterDTO = new MasterBlockDTO();
        //            objBlockMasterDTO.BlockCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][0]);
        //            objBlockMasterDTO.BlockName = dsPGSqlDataSet.Tables[0].Rows[i][1].ToString();
        //            objBlockMasterDTO.DistrictCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][2]);
        //            objBlockMasterDTO.StateCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][3]);
        //            objListBlockMasterDTO.Add(objBlockMasterDTO);
        //        }
        //        #endregion
        //        return objListBlockMasterDTO;
        //    }
        //    catch (NpgsqlException Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objPopulateBlockException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DPopulateBlock, "Error While Populating the Block....", Ex);
        //        throw objPopulateBlockException;
        //    }

        //    finally
        //    {
        //        trnPGSqlTransaction.Commit();
        //        conPGSqlConnection.Close();
        //    }
        //}
        //#endregion

        //#region Function Populate Gram-Panchayats
        //public List<GramPanchayatDTO> PopulateGramPanchayats(Int64 BlockCode)
        //{
        //    NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //    conPGSqlConnection.Open();
        //    NpgsqlTransaction trnPGSqlTransaction = trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //    try
        //    {

        //        #region Variable Declaration
        //        // NpgsqlConnection conPGSqlConnection = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
        //        NpgsqlCommand cmdPGSqlCommand = new NpgsqlCommand();
        //        DataSet dsPGSqlDataSet = new DataSet();
        //        NpgsqlDataAdapter adptPGSqlDataAdptr;
        //        //  NpgsqlTransaction trnPGSqlTransaction;
        //        List<GramPanchayatDTO> objListGramMasterDTO = new List<GramPanchayatDTO>();
        //        int intListCount = 0;
        //        #endregion
        //        #region Set Connection and Open it and Execute Stored Procedure
        //        // conPGSqlConnection.Open();
        //        //trnPGSqlTransaction = conPGSqlConnection.BeginTransaction();
        //        cmdPGSqlCommand.CommandType = CommandType.StoredProcedure;
        //        cmdPGSqlCommand.Connection = conPGSqlConnection;
        //        cmdPGSqlCommand.CommandText = "nlma.ufn_gf_populategrampanchayats";

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
        //        cmdPGSqlCommand.Parameters[0].Value = BlockCode;

        //        cmdPGSqlCommand.Parameters.Add(new NpgsqlParameter());
        //        cmdPGSqlCommand.Parameters[1].NpgsqlDbType = NpgsqlDbType.Refcursor;


        //        adptPGSqlDataAdptr = new NpgsqlDataAdapter(cmdPGSqlCommand);
        //        adptPGSqlDataAdptr.Fill(dsPGSqlDataSet);
        //        //trnPGSqlTransaction.Commit();
        //        // conPGSqlConnection.Close();

        //        #endregion
        //        #region Fill List
        //        intListCount = dsPGSqlDataSet.Tables[0].Rows.Count;
        //        for (int i = 0; i < intListCount; i++)
        //        {
        //            GramPanchayatDTO objGramMasterDTO = new GramPanchayatDTO();
        //            objGramMasterDTO.GramPanchayatCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][0]);
        //            objGramMasterDTO.GramPanchayatName = dsPGSqlDataSet.Tables[0].Rows[i][1].ToString();
        //            objGramMasterDTO.BlockCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][2]);
        //            objGramMasterDTO.DistrictCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][3]);
        //            objGramMasterDTO.StateCode = Convert.ToInt32(dsPGSqlDataSet.Tables[0].Rows[i][4]);
        //            objListGramMasterDTO.Add(objGramMasterDTO);
        //        }
        //        #endregion
        //        return objListGramMasterDTO;
        //    }
        //    catch (NpgsqlException Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objPopulateGramPanchayatsException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DPopulateGramPanchayats, "Error While Populating the GramPanchayats....", Ex);
        //        throw objPopulateGramPanchayatsException;
        //    }

        //    finally
        //    {
        //        trnPGSqlTransaction.Commit();
        //        conPGSqlConnection.Close();
        //    }
        //}
        //#endregion

        //public List<SelectListItem> GetStateByDistrict(long DistrictCode)
        //{
        //    NpgsqlConnection conn = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);

        //    conn.Open();
        //    NpgsqlTransaction trans = conn.BeginTransaction();

        //    try
        //    {
        //        NpgsqlCommand command = new NpgsqlCommand("nlma.ufn_gf_get_state_from_district", conn);
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters[0].Value = DistrictCode;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[1].NpgsqlDbType = NpgsqlDbType.Refcursor;

        //        NpgsqlDataReader dr = command.ExecuteReader();
        //        List<SelectListItem> lstState = new List<SelectListItem>();

        //        while (dr.Read())
        //        {
        //            lstState.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr.GetValue(1).ToString().Trim().ToLower()), Value = dr.GetValue(0).ToString().Trim().ToString(), Selected = true });
        //        }
        //        dr.Close();
        //        return (lstState);
        //    }
        //    catch (Exception Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objGetStateByDistrictException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DGetStateByDistrict, "Error While Getting State By District....", Ex);
        //        throw objGetStateByDistrictException;
        //    }
        //    finally
        //    {
        //        trans.Commit();
        //        conn.Close();
        //    }
        //}

        //public int InsertActivityLogDetails(int UserId, Int16 LayerType, string Activity, string Description, string Exception, string pageUrl, Int16 TypeLog, string IPAddress)
        //{
        //    NpgsqlConnection conn = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);


        //    conn.Open();
        //    NpgsqlTransaction trans = conn.BeginTransaction();

        //    try
        //    {
        //        NpgsqlCommand command = new NpgsqlCommand("nlma.ufn_log_user_activity_details", conn);
        //        command.CommandType = CommandType.StoredProcedure;
                    
        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters[0].Value = UserId;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[1].NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters[1].Value = LayerType;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[2].NpgsqlDbType = NpgsqlDbType.Varchar;
        //        command.Parameters[2].Value = Activity;


        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[3].NpgsqlDbType = NpgsqlDbType.Varchar;
        //        command.Parameters[3].Value = Description;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[4].NpgsqlDbType = NpgsqlDbType.Varchar;
        //        command.Parameters[4].Value = Exception;

        //        string[] strURLSplit = pageUrl.Split('/');
        //        string strURL = strURLSplit[0].ToString().Trim() + "//" + strURLSplit[2].ToString().Trim() + "/" + strURLSplit[3].ToString().Trim() + "/" + strURLSplit[4].ToString().Trim();


        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[5].NpgsqlDbType = NpgsqlDbType.Varchar;
        //        command.Parameters[5].Value = strURL;

        //        ////string[] actionControllerList = ctx.Request.Url.AbsolutePath.ToString().Trim().Split('/');
        //        ////objAuthorizationDTO.ActionURL = ctx.Request.Url.AbsolutePath;


        //        ////objAuthorizationDTO.ControllerName = actionControllerList[1].ToString().Trim();
        //        ////objAuthorizationDTO.ActionName = actionControllerList[2].ToString().Trim();



        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[6].NpgsqlDbType = NpgsqlDbType.Integer;
        //        command.Parameters[6].Value = TypeLog;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[7].NpgsqlDbType = NpgsqlDbType.Varchar;
        //        //command.Parameters[7].Value = IPAddress;
        //        command.Parameters[7].Value = IPAddress == "::1" ? "127.0.0.1" : IPAddress;

        //        int cnt = command.ExecuteNonQuery();
        //        return cnt;

        //        //return 1;

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //    finally
        //    {
        //        trans.Commit();
        //        conn.Close();
        //    }

        //}


        //public string GetLocationName(long LocationCode)
        //{
        //    string LocationName = string.Empty;
        //    NpgsqlConnection conn = new NpgsqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);


        //    conn.Open();
        //    NpgsqlTransaction trans = conn.BeginTransaction();

        //    try
        //    {
        //        NpgsqlCommand command = new NpgsqlCommand("nlma.ufn_gf_get_location_name", conn);
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.Add(new NpgsqlParameter());
        //        command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Bigint;
        //        command.Parameters[0].Value = LocationCode;

        //        NpgsqlDataReader dr = command.ExecuteReader();

        //        while (dr.Read())
        //        {
        //            LocationName = dr.GetValue(0).ToString().Trim();

        //        }
        //        dr.Close();
        //        return (LocationName);
        //    }
        //    catch (Exception Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objGetStateByDistrictException = new  PMGSY.DAL.Common.DALException("Get Location Name", "Error While Getting State Location Name....", Ex);
        //        throw objGetStateByDistrictException;
        //    }
        //    finally
        //    {
        //        trans.Commit();
        //        conn.Close();
        //    }

        //}

        //public Int16 GetLocationLevelFromLocationCode(Int32 intLocationCode)
        //{
        //    Int16 smallLocationLevel = 0;

        //    try
        //    {
        //        NpgsqlParameter[] parameters = new NpgsqlParameter[1];
        //        parameters[0] = new NpgsqlParameter("@param_int_location_code", NpgsqlDbType.Integer);
        //        parameters[0].Value = intLocationCode;

        //        smallLocationLevel = Convert.ToInt16(DataMethodPostgres.DataMethodsPgres.ExecuteScalar(CommandType.StoredProcedure, "nlma.ufn_gf_get_location_level_from_location_code", parameters));

        //        return smallLocationLevel;
        //    }
        //    catch (Exception Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objDALcException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DGetLocationLevelFromLocationCode, "Error while retrieving location level code from location code", Ex);
        //        throw objDALcException;
        //    }
        //}

        //public Int32 GetHigherLocationId(Int32 intLocationCode)
        //{
        //    Int32 intHigerLocationId = 0;

        //    try
        //    {
        //        NpgsqlParameter[] parameters = new NpgsqlParameter[1];
        //        parameters[0] = new NpgsqlParameter("@param_int_location_code", NpgsqlDbType.Integer);
        //        parameters[0].Value = intLocationCode;

        //        intHigerLocationId = Convert.ToInt32(DataMethodPostgres.DataMethodsPgres.ExecuteScalar(CommandType.StoredProcedure, "nlma.ufn_gf_get_higher_location", parameters));

        //        return intHigerLocationId;
        //    }
        //    catch (Exception Ex)
        //    {
        //         PMGSY.DAL.Common.DALException objDALcException = new  PMGSY.DAL.Common.DALException( PMGSY.Common.ModuleFunction.DGetHigherLocationId, "Error while retrieving higher location id", Ex);
        //        throw objDALcException;
        //    }
        //}

       

    }
    public interface IGeneralFunctionsDAL
    {
        //List<FinancialYearDTO> GetFinancialYear(char level, int StateCode, int? DistrictCode, int? BlockCode, long? GramPanchayatCode);
        //List<StateMasterDTO> PopulateState();
        //List<DistrictMasterDTO> PopulateDistrict(Int32 StateCode);
        //List<MasterBlockDTO> PopulateBlock(Int32 DistrictCode);
        //List<GramPanchayatDTO> PopulateGramPanchayats(Int64 BlockCode);

        //List<SelectListItem> GetStateByDistrict(long DistrictCode);
        //int InsertActivityLogDetails(int UserId, Int16 LayerType, string Activity, string Description, string Exception, string PageUrl, Int16 TypeLog, string IPAddress);

        //string GetLocationName(long LocationCode);

        //Int16 GetLocationLevelFromLocationCode(Int32 intLocationCode);
        //Int32 GetHigherLocationId(Int32 intLocationCode);
        //Boolean IsUserAuthorisedToViewReport( PMGSY.DTO.Common.SessionDTO objUser, Int32 intRequestedLocCode, Int32 intRequestedAgnCode);
    }
}
