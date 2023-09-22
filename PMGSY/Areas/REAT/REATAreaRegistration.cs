using System.Web.Mvc;

namespace PMGSY.Areas.REAT
{
    public class REATAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "REAT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "REAT_default",
                "REAT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
