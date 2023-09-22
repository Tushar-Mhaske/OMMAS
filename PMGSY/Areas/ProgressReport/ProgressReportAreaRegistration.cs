using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport
{
    public class ProgressReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProgressReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProgressReport_default",
                "ProgressReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
