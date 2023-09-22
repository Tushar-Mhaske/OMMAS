#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: ExistingRoadsBAL.cs

 * Author : Abhishek Kamble(changes done by Vikram Nandanwar)
 
 * Creation Date :24/May/2013

 * Desc : This class is used as BAL to call methods present in the DAL for Save,Edit,Update,Delete and listing of Existing Roads screens.  
 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PMGSY.DAL.ExistingRoads;
using PMGSY.Models;
using PMGSY.Models.ExistingRoads;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.BAL.ExistingRoads
{
    public class ExistingRoadsBAL : IExistingRoadasBAL
    {
        IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
        private PMGSYEntities db = new PMGSYEntities();

        #region Existing Roads BAL defination

        public bool AddExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message)
        {
            return objDAL.AddExistingRoads(existingRoadsViewModel, ref message);
        }

        public bool EditExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message)
        {
            return objDAL.EditExistingRoads(existingRoadsViewModel, ref message);
        }

        public Boolean DeleteExistingRoads(int _roadCode, ref string message)
        {
            return objDAL.DeleteExistingRoads(_roadCode, ref message);
        }


        public Array ListExistingRoads(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            return objDAL.ListExistingRoads(stateCode, districtCode, blockCode, categoryCode, ownerCode, page, rows, sidx, sord, out totalRecords, filters);
        }


        public Array GetCBRListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetCBRListDAL(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        public Array GetSurfaceList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetSurfaceList(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        public Array GetTrafficListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetTrafficListDAL(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        public Array GetCdWorkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetCdWorkList(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        public Array GetHabitationList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetHabitationList(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        public ExistingRoadsViewModel GetExistingRoads_ByRoadCode(int _roadCode)
        {
            return objDAL.GetExistingRoads_ByRoadCode(_roadCode);
        }


        public string GetExistingRoadCheckBAL(int MAST_ER_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                if (db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_IS_BENEFITTED_HAB).First() == "Y")
                {
                    if (!db.MASTER_ER_HABITATION_ROAD.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        return "Habitation Details are not added against Existing Road, Please Add Habitation Details.";
                    }
                }


                
                // For PMGSY II  do not appy below validations for finalizing DRRP

                if (PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme != 2)
                {

                    // check for Traffic Intensity 
                    if (!db.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        return "Traffic Intensity Details are not added against Existing Road, Please Add Traffic Intensity Details.";
                    }

                    //Check For CBR Details
                    decimal EnteredSegmentLength = 0;

                    decimal TotalLengthOfRoad = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First();


                    if (db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {

                        EnteredSegmentLength = db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_END_CHAIN - m.MAST_STR_CHAIN);
                        if (EnteredSegmentLength < TotalLengthOfRoad)
                        {
                            return " Please Enter Complete CBR Details.\n Total Entered Segment Length in CBR Details : " + EnteredSegmentLength + " Kms.\n Total Length Of Road : " + TotalLengthOfRoad + " Kms.";
                        }
                    }
                    else
                    {
                        return "CBR Details are not added against Existing Road, Please Add CBR Details.";
                    }


                    //Check For Surface Type Details
                    decimal EnteredSurfaceLength = 0;

                    if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        EnteredSurfaceLength = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_ER_SURFACE_LENGTH);

                        if (EnteredSurfaceLength < TotalLengthOfRoad)
                        {
                            return " Please Enter Complete Surface Type Details.\n Total entered Surface Length In Surface Details : " + EnteredSurfaceLength + " Kms.\nTotal Length of Road : " + TotalLengthOfRoad + " Kms.";
                        }
                    }
                    else
                    {
                        return "Surface Type Details are not added against Existing Road, Please Add Surface Type Details.";
                    }


                }




                return String.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An error occured while proccessing your request";
            }
            finally
            {
                db.Dispose();
            }

        }

        public string FinalizeExistingRoad(int MAST_ER_ROAD_CODE)
        {
            return objDAL.FinalizeExistingRoad(MAST_ER_ROAD_CODE);
        }

        public bool ValidateCoreNetwork(int existingRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == existingRoadCode))
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
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetCoreNetworkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            return objDAL.GetCoreNetworkList(page, rows, sidx, sord, out totalRecords, roadCode);
        }

        #endregion Existing RoadsBAL defination

        #region Traffic Intensity defination

        public bool AddTrafficIntensity(TrafficViewModel trafficViewModel, ref string message)
        {
            return objDAL.AddTrafficIntensity(trafficViewModel, ref message);
        }

        public bool EditTrafficIntensity(TrafficViewModel trafficViewModel, ref string message)
        {
            return objDAL.EditTrafficIntensity(trafficViewModel, ref message);
        }

        public Boolean DeleteTrafficIntensity(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR, ref string message)
        {
            return objDAL.DeleteTrafficIntensity(MAST_ER_ROAD_CODE, MAST_TI_YEAR, ref message);
        }

        public TrafficViewModel GetTrafficIntensity_ByRoadCode(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR)
        {
            return objDAL.GetTrafficIntensity_ByRoadCode(MAST_ER_ROAD_CODE, MAST_TI_YEAR);
        }

        #endregion Traffic Intensity defination

        #region CBR Value defination

        public bool AddCbrValue(CBRViewModel CbrViewModel, ref string message)
        {
            return objDAL.AddCbrValue(CbrViewModel, ref message);
        }

        public bool EditCbrValue(CBRViewModel CbrViewModel, ref string message)
        {
            return objDAL.EditCbrValue(CbrViewModel, ref message);
        }

        public Boolean DeleteCbrValue(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO, ref string message)
        {
            return objDAL.DeleteCbrValue(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO, ref message);
        }

        public CBRViewModel GetCBRDetails(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO)
        {
            return objDAL.GetCBRDetails(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO);
        }

        #endregion CBR Value defination

        #region CdWorks defination

        public bool AddCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            return objDAL.AddCDWorksDetails(cdWorksViewModel, ref message);
        }

        public bool EditCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            return objDAL.EditCDWorksDetails(cdWorksViewModel, ref message);
        }

        public Boolean DeleteCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER, ref string message)
        {
            return objDAL.DeleteCDWorksDetails(MAST_ER_ROAD_CODE, MAST_CD_NUMBER, ref message);
        }

        public CdWorksViewModel GetCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER)
        {
            return objDAL.GetCDWorksDetails(MAST_ER_ROAD_CODE, MAST_CD_NUMBER);
        }

        #endregion CdWorks defination

        #region Surface Type BAL defination

        public bool AddSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            return objDAL.AddSurfaceDetails(SurfaceViewModel, ref message);
        }

        public bool EditSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            return objDAL.EditSurfaceDetails(SurfaceViewModel, ref message);
        }

        public Boolean DeleteSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO, ref string message)
        {
            return objDAL.DeleteSurfaceDetails(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO, ref message);
        }

        public SurfaceTypeViewModel GetSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO)
        {
            return objDAL.GetSurfaceDetails(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO);
        }

        #endregion Surface Type BAL defination

        #region Mapped Habitation

        public Array GetHabitationListToMap(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetHabitationListToMap(roadCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAllHabitationList(int habCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetAllHabitationList(habCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool MapHabitationToRoad(string habCode, string roadCode)
        {
            return objDAL.MapHabitationToRoad(habCode, roadCode);
        }

        public bool DeleteMapHabitation(int habitationCode, int roadCode, out string message)
        {
            return objDAL.DeleteMapHabitation(habitationCode, roadCode, out message);
        }

        #endregion Mapped Habitation

        #region Map DRRP for PMGSY 1

        public bool MapDRRPPMGSY1RoadsBAL(int erRoadCode, int erRoadCode1, ref string message)
        {
            return objDAL.MapDRRPPMGSY1RoadsDAL(erRoadCode, erRoadCode1, ref message);
        }

        public Array GetMappedDRRPPmgsy1ListBAL(int block, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetMappedDRRPPmgsy1ListDAL(block, erRoadCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool UnMapDRRPPMGSY1RoadsBAL(int roadCode, ref string message)
        { 
            return objDAL.UnMapDRRPPMGSY1RoadsDAL(roadCode, ref message);
        }
        #endregion

        #region DRRP - II and PMGSY-I Mapping



        public Array GetProposalsForDRRPMappingUnderPMGSY3BAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            return objDAL.GetProposalsForDRRPMappingUnderPMGSY3DAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, adminCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        }

        public bool MapDRRPDetailsBAL(MapDRRPUnderPMGSY3 model)
        {
            return objDAL.MapDRRPDetailsDAL(model);
        }

        #endregion

        #region Trace Map
        public Array GetBlockListTM(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            return objDAL.GetDistrictListDAL(page, rows, sidx, sord, out totalRecords, blockcode);
        }

        public string TraceMapsSaveFileDetails(List<FileUploadModel> lstFileUploadViewModel, ref string filename)
        {
            objDAL = new ExistingRoadsDAL();
            try
            {
                return objDAL.TraceMapsSaveFileDetailsDAL(lstFileUploadViewModel, ref filename);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
            }
        }


        #endregion

        #region List ER For ITNO
        public Array ListExistingRoadsITNO(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            return objDAL.ListExistingRoadsITNO(stateCode, districtCode, blockCode, categoryCode, ownerCode, page, rows, sidx, sord, out totalRecords, filters);
        }
        #endregion


        #region List ER For ITNO For Inactive Blocks
        public Array ListExistingRoadsITNOForInactiveBlocks(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            return objDAL.ListExistingRoadsITNOForInactiveBlocksDAL(stateCode, districtCode, blockCode, categoryCode, ownerCode, page, rows, sidx, sord, out totalRecords, filters);
        }
        #endregion

        #region Not Feasible Roads under PMGSY III
        public Array GetNotFeasibleRoadsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            return objDAL.GetNotFeasibleRoadsListDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, adminCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        }

        public bool MapRoadDetailsBAL(MapNotFeasibleRoads model)
        {
            return objDAL.MapRoadDetailsDAL(model);
        }

        #endregion


        #region  Existing Road Shift
        public Array ListExistingRoadsForShiftBAL(int stateCode, int districtCode, int blockCode, int SchemeCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            return objDAL.ListExistingRoadsForShift(stateCode, districtCode, blockCode, SchemeCode, ownerCode, page, rows, sidx, sord, out totalRecords, filters);
        }
        #endregion
    }

    public interface IExistingRoadasBAL
    {
        #region Existing Roads BAL declaration

        bool AddExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message);

        bool EditExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message);

        Boolean DeleteExistingRoads(int _roadCode, ref string message);

        Array ListExistingRoads(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);

        Array GetCBRListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetSurfaceList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetTrafficListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetCdWorkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetHabitationList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        ExistingRoadsViewModel GetExistingRoads_ByRoadCode(int _roadCode);

        string GetExistingRoadCheckBAL(int MAST_ER_ROAD_CODE);

        string FinalizeExistingRoad(int MAST_ER_ROAD_CODE);

        bool ValidateCoreNetwork(int existingRoadCode);

        Array GetCoreNetworkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        #endregion Existing Roads BAL declaration

        #region Traffic Intensity BAL declaration

        bool AddTrafficIntensity(TrafficViewModel trafficViewModel, ref string message);
        bool EditTrafficIntensity(TrafficViewModel trafficViewModel, ref string message);
        Boolean DeleteTrafficIntensity(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR, ref string message);
        TrafficViewModel GetTrafficIntensity_ByRoadCode(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR);

        #endregion Traffic Intensity BAL declaration

        #region CBR Value Intensity BAL

        bool AddCbrValue(CBRViewModel CbrViewModel, ref string message);
        bool EditCbrValue(CBRViewModel CbrViewModel, ref string message);
        Boolean DeleteCbrValue(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO, ref string message);
        CBRViewModel GetCBRDetails(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO);

        #endregion CBR Value Intensity BAL

        #region CdWorks BAL

        bool AddCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message);
        bool EditCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message);
        Boolean DeleteCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER, ref string message);
        CdWorksViewModel GetCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER);

        #endregion CdWorks BAL

        #region Surface Type BAL declaration

        bool AddSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message);
        bool EditSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message);
        Boolean DeleteSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO, ref string message);
        SurfaceTypeViewModel GetSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO);

        #endregion Surface Type BAL declaration

        #region Mapped Habitation

        Array GetHabitationListToMap(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAllHabitationList(int habCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool MapHabitationToRoad(string habCode, string roadCode);

        bool DeleteMapHabitation(int habitationCode, int roadCode, out string message);

        #endregion Mapped Habitation

        #region Map DRRP for PMGSY 1

        bool MapDRRPPMGSY1RoadsBAL(int erRoadCode, int erRoadCode1, ref string message);
        Array GetMappedDRRPPmgsy1ListBAL(int block, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UnMapDRRPPMGSY1RoadsBAL(int roadCode, ref string message);
        #endregion

        #region  DRRP - II and PMGSY-I Mapping
        Array GetProposalsForDRRPMappingUnderPMGSY3BAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        bool MapDRRPDetailsBAL(MapDRRPUnderPMGSY3 model);

        #endregion

        #region TraceMap
        Array GetBlockListTM(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode);
        string TraceMapsSaveFileDetails(List<FileUploadModel> lst_files, ref string filename);

        #endregion

        #region List Existing Roads For ITNO
        Array ListExistingRoadsITNO(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);
        #endregion


        #region List Existing Roads For ITNO For Inactive Blocks
        Array ListExistingRoadsITNOForInactiveBlocks(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);
        #endregion

        #region Not Feasible Roads under PMGSY III
        Array GetNotFeasibleRoadsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        // MapRoadDetailsBAL
        bool MapRoadDetailsBAL(MapNotFeasibleRoads model);


        #endregion

        #region  Existing Road Shift
        Array ListExistingRoadsForShiftBAL(int stateCode, int districtCode, int blockCode, int SchemeCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);
        #endregion
    }
}