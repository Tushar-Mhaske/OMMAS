using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.NIT;
using PMGSY.Models.NIT;

namespace PMGSY.BAL.NIT
{
    public class NITBAL:INITBAL
    {
        INITDAL objNITDAL = new NITDAL();

        public Array GetNITDetailsBAL(int stateCode, int districtCode, int adminNDCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objNITDAL.GetNITDetailsDAL(stateCode, districtCode, adminNDCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveNITDetailsBAL(NITDetails objNITDetails, ref string message)
        {
            return objNITDAL.SaveNITDetailsDAL(objNITDetails, ref message);
        }


        public NITDetails GetNITDetailsBAL(int tendNITCode)
        {
            return objNITDAL.GetNITDetailsDAL(tendNITCode);
        }

        public bool UpdateNITDetailsBAL(NITDetails objNITDetails, ref string message)
        {
            return objNITDAL.UpdateNITDetailsDAL(objNITDetails, ref message);
        }


        public bool DeleteNITDetailsBAL(int tendNITCode, ref string message)
        {
            return objNITDAL.DeleteNITDetailsDAL(tendNITCode, ref message);
        }


        public bool PublishNITBAL(int tendNITCode)
        {
            return objNITDAL.PublishNITDAL(tendNITCode);
        }


        public Array GetNITRoadDetailsListBAL(int TendNITCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objNITDAL.GetNITRoadDetailsListDAL(TendNITCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveNITRoadDetailsBAL(NITRoadDetails objNITRoadDetails, ref string message)
        {
            return objNITDAL.SaveNITRoadDetailsDAL(objNITRoadDetails, ref message);
        }


        public NITRoadDetails GetNITRoadDetailsBAL(int tendNITID, int tendNITCode, bool isView = false)
        {
            return objNITDAL.GetNITRoadDetailsDAL(tendNITID, tendNITCode, isView);
        }


        public bool UpdateNITRoadDetailsBAL(NITRoadDetails objNITRoadDetails, ref string message)
        {
            return objNITDAL.UpdateNITRoadDetailsDAL(objNITRoadDetails, ref message);
        }


        public bool DeleteNITRoadDetailsBAL(int tendNITID, int tendNITCode, ref string message)
        {
            return objNITDAL.DeleteNITRoadDetailsDAL(tendNITID,tendNITCode, ref message);
        }


        public void GetEstimatedCostMaintenanceCostBAL(int roadCode,int workCode, ref string totalEstimatedCost, ref string totalMaintenanceCost)
        {
            objNITDAL.GetEstimatedCostMaintenanceCostDAL(roadCode, workCode, ref totalEstimatedCost, ref totalMaintenanceCost);
        }
    }
    public interface INITBAL
    {

        Array GetNITDetailsBAL(int stateCode, int districtCode, int adminNDCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveNITDetailsBAL(NITDetails objNITDetails, ref string message);

        NITDetails GetNITDetailsBAL(int tendNITCode);

        bool UpdateNITDetailsBAL(NITDetails objNITDetails, ref string message);

        bool DeleteNITDetailsBAL(int tendNITCode, ref string message);

        bool PublishNITBAL(int tendNITCode);

        Array GetNITRoadDetailsListBAL(int TendNITCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveNITRoadDetailsBAL(NITRoadDetails objNITRoadDetails, ref string message);

        NITRoadDetails GetNITRoadDetailsBAL(int tendNITID, int tendNITCode, bool isView = false);

        bool UpdateNITRoadDetailsBAL(NITRoadDetails objNITRoadDetails, ref string message);

        bool DeleteNITRoadDetailsBAL(int tendNITID, int tendNITCode, ref string message);

        void GetEstimatedCostMaintenanceCostBAL(int roadCode,int workCode, ref string totalEstimatedCost, ref string totalMaintenanceCost);
    }
}