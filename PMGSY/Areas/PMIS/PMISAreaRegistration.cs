using System.Web.Mvc;

namespace PMGSY.Areas.PMIS
{
    public class PMISAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PMIS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PMIS_default",
                "PMIS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
