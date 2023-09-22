using System.Web.Mvc;

namespace PMGSY.Areas.RESTAPIImplementation
{
    public class RESTAPIImplementationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RESTAPIImplementation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RESTAPIImplementation_default",
                "RESTAPIImplementation/{controller}/{action}/{id}/{id1}/{id2}",
                new { action = "Index", id = UrlParameter.Optional, id1 = UrlParameter.Optional, id2 = UrlParameter.Optional }
            );
        }
    }
}
