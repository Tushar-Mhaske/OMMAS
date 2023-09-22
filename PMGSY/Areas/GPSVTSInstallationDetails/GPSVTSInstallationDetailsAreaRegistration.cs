using System.Web.Mvc;

namespace PMGSY.Areas.GPSVTSInstallationDetails
{
    public class GPSVTSInstallationDetailsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GPSVTSInstallationDetails";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GPSVTSInstallationDetails_default",
                "GPSVTSInstallationDetails/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
