using PMGSY.DAL.TourClaim;
using System;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.BAL.TourClaim
{
    public class TourClaimBAL : ITourClaimBAL
    {
        ITourClaimDAL objDAL;

        #region NQM Tour Claim

        public Array GetNQMCurrScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            objDAL = new TourClaimDAL();
            return objDAL.GetNQMCurrScheduleListDAL(month, year, page, rows, sidx, sord, out totalRecords);
        }

        public bool InsertTourClaimDetailsBAL(FormCollection formCollection, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertTourClaimDetailsDAL(formCollection, out IsValid);
        }

        public bool FinalizeTourDetailBAL(int scheduleCode, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.FinalizeTourDetailDAL(scheduleCode, out IsValid);
        }

        public bool InsertDistrictDetailsBAL(FormCollection formCollection, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertDistrictDetailsDAL(formCollection, out IsValid);
        }

        public Array GetTourDistrictListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTourDistrictListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool InsertTravelClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertTravelClaimDetailsDAL(formCollection, boardingPass, travelTicket, out IsValid);
        }

        public Array GetTravelClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTravelClaimListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateTravelClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdateTravelClaimDetailsDAL(formCollection, boardingPass, travelTicket, out IsValid);
        }

        public bool InsertLodgeClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertLodgeClaimDetailsDAL(formCollection, bill, receipt, gBill, out IsValid);
        }

        public Array GetLodgeClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetLodgeClaimListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateLodgeClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdateLodgeClaimDetailsDAL(formCollection, bill, receipt, gBill, out IsValid);
        }

        public bool InsertInspectionHonorariumBAL(FormCollection formCollection, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertInspectionHonorariumDAL(formCollection, out IsValid);
        }

        public Array GetInspectionHonorariumListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetInspectionHonorariumListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateInspectionHonorariumBAL(FormCollection formCollection, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdateInspectionHonorariumDAL(formCollection, out IsValid);
        }

        public bool InsertMeetingHonorariumBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertMeetingHonorariumDAL(formCollection, postedBgFile, postedBgFile1, postedBgFile2, out IsValid);
        }

        public Array GetMeetingHonorariumListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetMeetingHonorariumListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateMeetingHonorariumBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdateMeetingHonorariumDAL(formCollection, postedBgFile, postedBgFile1, postedBgFile2, out IsValid);
        }

        public bool InsertMiscellaneousClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertMiscellaneousClaimDetailsDAL(formCollection, postedBgFile, out IsValid);
        }

        public Array GetMiscellaneousClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetMiscellaneousClaimListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdateMiscellaneousClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdateMiscellaneousClaimDetailsDAL(formCollection, postedBgFile, out IsValid);
        }

        public bool InsertPermissionClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.InsertPermissionClaimDetailsDAL(formCollection, postedBgFile, out IsValid);
        }

        public Array GetPermissionClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetPermissionClaimListDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool UpdatePermissionClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.UpdatePermissionClaimDetailsDAL(formCollection, postedBgFile, out IsValid);
        }

        #endregion

        #region CQC Tour Claim

        public Array GetTourDistrictListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTourDistrictListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool FinalizeSanctionTourDetailsBAL(string id, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.FinalizeSanctionTourDetailsDAL(id, out IsValid);
        }

        public bool ApproveTourDetailsBAL(string id, out String IsValid)
        {
            objDAL = new TourClaimDAL();
            return objDAL.ApproveTourDetailsDAL(id, out IsValid);
        }

        public Array GetTravelClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTravelClaimListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetLodgeClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetLodgeClaimListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetInspectionHonorariumListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetInspectionHonorariumListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetMeetingHonorariumListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetMeetingHonorariumListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array LoadMiscellaneousClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.LoadMiscellaneousClaimListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        
        public Array LoadPermissionClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.LoadPermissionClaimListCqcDAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion

        #region Finance Tour Claim

        public Array GetFinanceMonitorListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            objDAL = new TourClaimDAL();
            return objDAL.GetFinanceMonitorListDAL(month, year, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetTourDistrictListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTourDistrictListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetTravelClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTravelClaimListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetLodgeClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetLodgeClaimListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetInspectionHonorariumListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetInspectionHonorariumListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetMeetingHonorariumListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetMeetingHonorariumListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array LoadMiscellaneousClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.LoadMiscellaneousClaimListFin1DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion

        #region Finance 2 Tour Claim

        public Array GetFinance2MonitorListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            objDAL = new TourClaimDAL();
            return objDAL.GetFinance2MonitorListDAL(month, year, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetTourDistrictListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTourDistrictListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetTravelClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetTravelClaimListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetLodgeClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetLodgeClaimListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetInspectionHonorariumListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetInspectionHonorariumListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetMeetingHonorariumListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.GetMeetingHonorariumListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array LoadMiscellaneousClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                objDAL = new TourClaimDAL();
                return objDAL.LoadMiscellaneousClaimListFin2DAL(scheduleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion
    }

    public interface ITourClaimBAL
    {
        #region NQM Tour Claim
        Array GetNQMCurrScheduleListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        bool InsertTourClaimDetailsBAL(FormCollection formCollection, out String IsValid);
        bool FinalizeTourDetailBAL(int scheduleCode, out String IsValid);
        bool InsertDistrictDetailsBAL(FormCollection formCollection, out String IsValid);
        Array GetTourDistrictListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool InsertTravelClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid);
        Array GetTravelClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateTravelClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid);
        bool InsertLodgeClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid);
        Array GetLodgeClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateLodgeClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid);
        bool InsertInspectionHonorariumBAL(FormCollection formCollection, out String IsValid);
        Array GetInspectionHonorariumListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateInspectionHonorariumBAL(FormCollection formCollection, out String IsValid);
        Array GetMeetingHonorariumListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool InsertMeetingHonorariumBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid);
        bool UpdateMeetingHonorariumBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid);
        bool InsertMiscellaneousClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        Array GetMiscellaneousClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateMiscellaneousClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        bool InsertPermissionClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        Array GetPermissionClaimListBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdatePermissionClaimDetailsBAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);

        #endregion

        #region CQC Tour Claim
        Array GetTourDistrictListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool FinalizeSanctionTourDetailsBAL(string id, out String IsValid);
        bool ApproveTourDetailsBAL(string id, out String IsValid);
        Array LoadMiscellaneousClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadPermissionClaimListCqcBAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Finance Tour Claim

        Array GetFinanceMonitorListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array GetTourDistrictListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadMiscellaneousClaimListFin1BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Finance 2 Tour Claim

        Array GetFinance2MonitorListBAL(int month, int year, int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array GetTourDistrictListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadMiscellaneousClaimListFin2BAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

    }
}