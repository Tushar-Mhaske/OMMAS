using PMGSY.DAL.SLSCGB;
using PMGSY.Models.SLSCGB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.SLSCGB
{
    public class SLSCGBBAL : ISLSCGBBAL
    {
        SLSCGBDAL objDAL = null;

        public bool AddSLSCGBBAL(SLSCGBViewModel model, ref string message)
        {
            objDAL = new SLSCGBDAL();
            return objDAL.AddSLSCGBDAL(model, ref message);
        }

        public Array GetMeetingListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        { 
            objDAL = new SLSCGBDAL();
            return objDAL.GetMeetingListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public string DeleteMeetingDetailsBAL(int meetingCode)
        { 
            objDAL = new SLSCGBDAL();
            return objDAL.DeleteMeetingDetailsDAL(meetingCode);
        }
    }

    public interface ISLSCGBBAL
    {
        bool AddSLSCGBBAL(SLSCGBViewModel model, ref string message);
        Array GetMeetingListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        string DeleteMeetingDetailsBAL(int meetingCode);
    }
}