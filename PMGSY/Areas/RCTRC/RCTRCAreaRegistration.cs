using System.Web.Mvc;

namespace PMGSY.Areas.RCTRC
{
    public class RCTRCAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RCTRC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RCTRC_default",
                "RCTRC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
