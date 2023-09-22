using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.StateLogin;

namespace PMGSY.BAL.StateLogin
{
    public class HabitationConnectivityBAL : IHabitationConnectivityBAL
    {
        public bool EditHabStatus(int habCode, ref string message)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.EditHabStatus(habCode, ref message);
        }

        public Array GetHabitationDetails(int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetHabitationDetails(districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAllDetails_ByBlockCode(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetAllDetails_ByBlockCode(blockCode,page,rows,sidx,sord,out totalRecords);
        }

        public Array GetTotalHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetTotalHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetConnectedHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetNotConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetNotConnectedHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetNotFeasibleHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetNotFeasibleHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetStateConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetStateConnectedHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetBenifitedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetBenifitedHabsDetails(blockCode, page, rows, sidx, sord, out totalRecords);
        }
        public Array GetHabsDetailsByStatusBAL(string flagParam, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            IHabitationConnectivityDAL objStateDAL = new HabitationConnectivityDAL();
            return objStateDAL.GetHabsDetailsByStatusDAL(flagParam,blockCode, page, rows, sidx, sord, out totalRecords);
   
        }

    }

    public interface IHabitationConnectivityBAL
    {
        bool EditHabStatus(int habCode, ref string message);

        Array GetHabitationDetails(int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAllDetails_ByBlockCode(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTotalHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetNotConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetNotFeasibleHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetStateConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBenifitedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabsDetailsByStatusBAL(string flagParam, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
    }
}