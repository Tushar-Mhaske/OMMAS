using System.Web.Mvc;

namespace PMGSY.Areas.LocationSSRSReports
{
    public class LocationSSRSReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LocationSSRSReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LocationSSRSReports_default",
                "LocationSSRSReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
