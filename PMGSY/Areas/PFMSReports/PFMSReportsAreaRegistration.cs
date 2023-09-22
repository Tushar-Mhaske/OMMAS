using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports
{
    public class PFMSReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PFMSReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PFMSReports_default",
                "PFMSReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
