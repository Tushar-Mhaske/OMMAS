using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Feedback.BAL
{
    public interface IDetailsFBBAL
    {
        Array FBListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int formonth, int foryear, int state, string category, string appr, string status, string fbthrough);
    }


    public class DetailsFBBAL : IDetailsFBBAL
    {
        Feedback.DAL.FeedbackDAL qualityDAL;

        public Array FBListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int formonth, int foryear, int state, string category, string appr, string status, string fbthrough)
        {
            qualityDAL = new DAL.FeedbackDAL();
            return qualityDAL.FBListDAL(page, rows, sidx, sord, out totalRecords, formonth, foryear, state, category.Trim(), appr.Trim(), status.Trim(), fbthrough.Trim());
        }
    }

    

}