using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.QMSSRSReports.DAL
{
    public class QMReportsDAL
    {
        private PMGSYEntities dbContext;

        public List<String> GetVisitImages(int VisitId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<String> lstFiles = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(x => x.MP_VISIT_ID == VisitId).Select(f => f.FILE_NAME+"$"+f.IS_PDF).ToList<String>();
                return lstFiles;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetVisitImages()");
                return null;
            }
        }
    }
}