using System.Web.Mvc;

namespace PMGSY.Areas.DARPAN2UP
{
    public class DARPAN2UPAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DARPAN2UP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DARPAN2UP_default",
                "DARPAN2UP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
