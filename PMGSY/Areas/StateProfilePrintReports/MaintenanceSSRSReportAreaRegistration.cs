using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport
{
    public class MaintenanceSSRSReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MaintenanceSSRSReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MaintenanceSSRSReport_default",
                "MaintenanceSSRSReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
