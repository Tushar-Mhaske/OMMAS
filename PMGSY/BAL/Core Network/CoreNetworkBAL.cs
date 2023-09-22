#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: CoreNetworkBAL.cs

 * Author : Vikram Nandanwar
 
 * Creation Date :24/May/2013

 * Desc : This class is used as BAL to call methods present in the DAL for Save,Edit,Update,Delete and listing of Core Network screens.  
 
 */
#endregion

using PMGSY.DAL.Core_Network;
using PMGSY.Models;
using PMGSY.Models.CoreNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.Core_Network
{
    public class CoreNetworkBAL : ICoreNetworkBAL
    {
        ICoreNetworkDAL objDAL = new CoreNetworkDAL();
        PMGSYEntities dbContext = new PMGSYEntities();

        #region CORE_NETWORK

        /// <summary>
        /// Returns the list of Core Network roads
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="roadType"></param>
        /// <param name="roadCode"></param>
        /// <param name="roadName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetWorksList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            try
            {
                return objDAL.GetCoreNetWorksList(stateCode, districtCode, blockCode, roadType, roadCode, roadName, page, rows, sidx, sord, out totalRecords, CnCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        /// <summary>
        /// saves the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddCoreNetworks(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.AddCoreNetworks(model, ref message);
        }

        /// <summary>
        /// update the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditCoreNetworks(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.EditCoreNetworks(model, ref message);
        }

        /// <summary>
        /// returns the core network details for updation
        /// </summary>
        /// <param name="networkCode"></param>
        /// <returns></returns>
        public CoreNetworkViewModel GetCoreNetworkDetails(int networkCode)
        {
            return objDAL.GetCoreNetworkDetails(networkCode);
        }

        /// <summary>
        /// delete the core network details
        /// </summary>
        /// <param name="networkCode"></param>
        /// <returns></returns>
        public bool DeleteCoreNetworks(int networkCode)
        {
            return objDAL.DeleteCoreNetworks(networkCode);
        }

        /// <summary>
        /// returns the list of habitations for populating the grid data
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetHabitationList(int habCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationList(habCode, flag, page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// map the habitation to the road
        /// </summary>
        /// <param name="habitationCode">habitation id</param>
        /// <param name="roadCode">core network id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool AddHabitation(int habitationCode, int roadCode, ref string message)
        {
            return objDAL.AddHabitation(habitationCode, roadCode, ref message);
        }

        /// <summary>
        /// delete the habitation map details from the road
        /// </summary>
        /// <param name="habitationCode">corresponding habitation id</param>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public bool DeleteMapHabitation(int habitationCode, string flag, int roadCode)
        {
            return objDAL.DeleteMapHabitation(habitationCode, flag, roadCode);
        }

        /// <summary>
        /// populates the availabe list of habitations to be mapped 
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMap(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationListToMap(roadCode, blockCode, erRoadCode, page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// maps the habitations to the current road
        /// </summary>
        /// <param name="encryptedHabCodes">encrypted habitation codes</param>
        /// <param name="roadName">core network code</param>
        /// <returns></returns>
        public bool MapHabitationToRoad(string habCode, string roadCode)
        {
            return objDAL.MapHabitationToRoad(habCode, roadCode);
        }

        /// <summary>
        /// save operation of file details
        /// </summary>
        /// <param name="list">list of file details</param>
        /// <returns>response message</returns>
        public string AddFileUploadDetailsBAL(List<CoreNetworkUploadFileViewModel> list, ref string message)
        {
            List<PLAN_ROAD_UPLOAD_FILE> files = new List<PLAN_ROAD_UPLOAD_FILE>();
            foreach (CoreNetworkUploadFileViewModel model in list)
            {
                files.Add(
                    new PLAN_ROAD_UPLOAD_FILE()
                    {
                        PLAN_CN_ROAD_CODE = model.PLAN_CN_ROAD_CODE,
                        PLAN_UPLOAD_DATE = DateTime.Now,
                        PLAN_FILE_NAME = model.name,
                        PLAN_START_CHAINAGE = model.PLAN_START_CHAINAGE,
                        PLAN_END_CHAINAGE = model.PLAN_END_CHAINAGE,
                        PLAN_FILE_SIZE = model.size
                    }
               );
            }
            return objDAL.AddFileUploadDetailsDAL(files);
        }

        /// <summary>
        /// populates the list of Files 
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no of total records</param>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int roadCode)
        {
            return objDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        /// <summary>
        /// Update the details associated with the file
        /// </summary>
        /// <param name="model">contains the updated file details </param>
        /// <returns></returns>
        public string UpdateFileDetailsBAL(CoreNetworkUploadFileViewModel model, ref string message)
        {
            PLAN_ROAD_UPLOAD_FILE files = new PLAN_ROAD_UPLOAD_FILE();
            files.PLAN_FILE_ID = Convert.ToInt32(model.PLAN_FILE_ID);
            files.PLAN_CN_ROAD_CODE = model.PLAN_CN_ROAD_CODE;
            files.PLAN_START_CHAINAGE = model.PLAN_START_CHAINAGE;
            files.PLAN_END_CHAINAGE = model.PLAN_END_CHAINAGE;

            return objDAL.UpdateFileDetailsDAL(files, ref message);
        }

        /// <summary>
        /// delete file operation
        /// </summary>
        /// <param name="files">model containing file details </param>
        /// <returns>message of operation</returns>
        public string DeleteFileDetails(int fileID, int roadCode, string filename)
        {
            PLAN_ROAD_UPLOAD_FILE files = dbContext.PLAN_ROAD_UPLOAD_FILE.Where(
                a => a.PLAN_FILE_ID == fileID &&
                a.PLAN_CN_ROAD_CODE == roadCode &&
                a.PLAN_FILE_NAME == filename).FirstOrDefault();

            return objDAL.DeleteFileDetails(files);
        }

        public string GetCoreNetworkChecksBAL(int roadCode)
        {
            try
            {
                //Allow finalization of CN without habitation finalization
                if (dbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == roadCode && a.PLAN_RD_ROUTE == "N").Any())
                {
                    if (dbContext.PLAN_ROAD_HABITATION.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Count() >= 2)
                        return "";
                    else
                        return "Map atleast 2 Habitations for Finalization of Missing Link";
                }
                // Check whether habitations are added 
                if (!dbContext.PLAN_ROAD_HABITATION.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Any())
                {

                    return "Habitation Details are not added against Core Network, Please Add Habitation Details.";

                }
                //if (!dbContext.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Any())
                //{
                //    return "File Details are not added against Core Network, Please Add File Details.";
                //}
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string FinalizeCoreNetworkBAL(int roadCode)
        {
            return objDAL.FinalizeCoreNetworkDAL(roadCode);
        }

        public bool CheckPavementLength(int roadCode, decimal chainage)
        {
            try
            {
                decimal? pavementLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Sum(m => (Decimal?)m.IMS_PAV_LENGTH);
                //if (pavementLength > chainage)
                ///Changes by SAMMED A. PATIL to edit CN length greater than or equal to sanction length
                if (pavementLength != null)
                {
                    if (chainage >= pavementLength)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the Proposal list associated with this core network
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListProposals(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.ListProposals(roadCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListCandidateRoads(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized)
        {
            return objDAL.ListCandidateRoads(roadCode, page, rows, sidx, sord, out totalRecords, out IsFinalized);
        }

        public bool MapCandidateRoad(CandidateRoadViewModel model, ref string message)
        {
            return objDAL.MapCandidateRoad(model, ref message);
        }

        public bool DeleteMappedDRRPDetails(int DRRPCode, int CNCode)
        {
            return objDAL.DeleteMappedDRRPDetails(DRRPCode, CNCode);
        }

        public bool FinalizeMappedDRRPDetails(int CNCode)
        {
            return objDAL.FinalizeMappedDRRPDetails(CNCode);
        }

        public Array ListDRRPMappedHabitations(int DRRPCode, int CNCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.ListDRRPMappedHabitations(DRRPCode, CNCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteMappedDRRPHabitation(int DRRPCode, int CNCode, int habCode)
        {
            return objDAL.DeleteMappedDRRPHabitation(DRRPCode, CNCode, habCode);
        }
        #endregion

        #region RCPLWE

        /// <summary>
        /// Returns the list of RCPLWE roads
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="roadType"></param>
        /// <param name="roadCode"></param>
        /// <param name="roadName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetRCPLWEList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CNCode)
        {
            try
            {
                return objDAL.GetRCPLWEList(stateCode, districtCode, blockCode, roadType, roadCode, roadName, page, rows, sidx, sord, out totalRecords, CNCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        /// <summary>
        /// saves the RCPLWE details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddRCPLWE(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.AddRCPLWE(model, ref message);
        }

        /// <summary>
        /// update the RCPLWE details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditRCPLWE(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.EditRCPLWE(model, ref message);
        }

        /// <summary>
        /// update the RCPLWE details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeleteRCPLWE(int networkCode)
        {
            return objDAL.DeleteRCPLWE(networkCode);
        }

        /// <summary>
        /// populates the availabe list of habitations to be mapped RCPLWE
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMapRCPLWE(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationListToMapRCPLWE(roadCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetHabitationListRCPLWE(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationListRCPLWE(roadCode, flag, page, rows, sidx, sord, out totalRecords);
        }

        public bool MapHabitationToRoadRCPLWE(string encryptedHabCodes, string roadName)
        {
            return objDAL.MapHabitationToRoadRCPLWE(encryptedHabCodes, roadName);
        }

        public string FinalizeCoreNetworkRCPLWEBAL(int roadCode)
        {
            return objDAL.FinalizeCoreNetworkRCPLWEDAL(roadCode);
        }
        #endregion

        #region Village Vibrant Programme

        /// <summary>
        /// Returns the list of Core Network roads
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="roadType"></param>
        /// <param name="roadCode"></param>
        /// <param name="roadName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetWorksListVVP(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            try
            {
                return objDAL.GetCoreNetWorksListVVP(stateCode, districtCode, blockCode, roadType, roadCode, roadName, page, rows, sidx, sord, out totalRecords, CnCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }


        /// <summary>
        /// populates the availabe list of habitations to be mapped 
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMapVVP(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationListToMapVVP(roadCode, blockCode, erRoadCode, page, rows, sidx, sord, out totalRecords);
        }


        /// <summary>
        /// saves the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddCoreNetworksVVP(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.AddCoreNetworksVVP(model, ref message);
        }

        /// <summary>
        /// update the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditCoreNetworksVVP(CoreNetworkViewModel model, ref string message)
        {
            return objDAL.EditCoreNetworksVVP(model, ref message);
        }


        public bool MapCandidateRoadVVP(CandidateRoadViewModel model, ref string message)
        {
            return objDAL.MapCandidateRoadVVP(model, ref message);
        }


        #endregion
    }

    public interface ICoreNetworkBAL
    {
        #region CORE_NETWORKS

        Array GetCoreNetWorksList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode);
        bool AddCoreNetworks(CoreNetworkViewModel model, ref string message);
        bool EditCoreNetworks(CoreNetworkViewModel model, ref string message);
        CoreNetworkViewModel GetCoreNetworkDetails(int networkCode);
        bool DeleteCoreNetworks(int networkCode);
        Array GetHabitationList(int habCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddHabitation(int habitationCode, int roadCode, ref string message);
        bool DeleteMapHabitation(int habitationCode, string flag, int roadCode);
        Array GetHabitationListToMap(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoad(string habCode, string roadCode);
        string AddFileUploadDetailsBAL(List<CoreNetworkUploadFileViewModel> list, ref string message);
        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int roadCode);
        string UpdateFileDetailsBAL(CoreNetworkUploadFileViewModel model, ref string message);
        string DeleteFileDetails(int fileID, int roadCode, string filename);
        string GetCoreNetworkChecksBAL(int roadCode);
        string FinalizeCoreNetworkBAL(int roadCode);
        bool CheckPavementLength(int proposalCode, decimal chainage);
        Array ListProposals(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListCandidateRoads(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized);
        bool MapCandidateRoad(CandidateRoadViewModel model, ref string message);
        bool DeleteMappedDRRPDetails(int DRRPCode, int CNCode);
        bool FinalizeMappedDRRPDetails(int CNCode);
        Array ListDRRPMappedHabitations(int DRRPCode, int CNCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteMappedDRRPHabitation(int DRRPCode, int CNCode, int habCode);

        #endregion

        #region RCPLWE
        Array GetRCPLWEList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CNCode);
        bool AddRCPLWE(CoreNetworkViewModel model, ref string message);
        bool EditRCPLWE(CoreNetworkViewModel model, ref string message);
        bool DeleteRCPLWE(int networkCode);

        Array GetHabitationListToMapRCPLWE(int cnRoadCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationListRCPLWE(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoadRCPLWE(string encryptedHabCodes, string roadName);

        string FinalizeCoreNetworkRCPLWEBAL(int roadCode);
        #endregion

        #region Village Vibrant Programme
        Array GetCoreNetWorksListVVP(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode);

        Array GetHabitationListToMapVVP(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool AddCoreNetworksVVP(CoreNetworkViewModel model, ref string message);

        bool EditCoreNetworksVVP(CoreNetworkViewModel model, ref string message);

        bool MapCandidateRoadVVP(CandidateRoadViewModel model, ref string message);

        #endregion
    }
}