using System.Web.Mvc;

namespace PMGSY.Areas.RESTImplementationForABAProgress
{
    public class RESTImplementationForABAProgressAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RESTImplementationForABAProgress";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RESTImplementationForABAProgress_default",
                "RESTImplementationForABAProgress/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
