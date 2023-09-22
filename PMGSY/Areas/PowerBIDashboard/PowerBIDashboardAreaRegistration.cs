using System.Web.Mvc;

namespace PMGSY.Areas.PowerBIDashboard
{
    public class PowerBIDashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PowerBIDashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PowerBIDashboard_default",
                "PowerBIDashboard/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
