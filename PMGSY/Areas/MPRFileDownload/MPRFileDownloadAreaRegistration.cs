using System.Web.Mvc;

namespace PMGSY.Areas.MPRFileDownload
{
    public class MPRFileDownloadAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MPRFileDownload";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MPRFileDownload_default",
                "MPRFileDownload/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
