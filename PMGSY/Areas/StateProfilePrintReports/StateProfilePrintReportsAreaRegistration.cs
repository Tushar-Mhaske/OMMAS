using System.Web.Mvc;

namespace PMGSY.Areas.StateProfilePrintReports
{
    public class StateProfilePrintReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StateProfilePrintReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StateProfilePrintReports_default",
                "StateProfilePrintReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
