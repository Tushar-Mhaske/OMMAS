using System.Web.Mvc;

namespace PMGSY.Areas.PRAYAS
{
    public class PRAYASAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PRAYAS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PRAYAS_default",
                "PRAYAS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
