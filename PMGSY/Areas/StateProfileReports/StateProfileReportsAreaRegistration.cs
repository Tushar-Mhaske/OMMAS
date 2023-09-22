using System.Web.Mvc;

namespace PMGSY.Areas.StateProfileReports
{
    public class StateProfileReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StateProfileReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StateProfileReports_default",
                "StateProfileReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
