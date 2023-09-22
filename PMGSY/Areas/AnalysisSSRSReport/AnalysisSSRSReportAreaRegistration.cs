using System.Web.Mvc;

namespace PMGSY.Areas.AnalysisSSRSReport
{
    public class AnalysisSSRSReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AnalysisSSRSReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AnalysisSSRSReport_default",
                "AnalysisSSRSReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
