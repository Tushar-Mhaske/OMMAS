using System.Web.Mvc;

namespace PMGSY.Areas.ErrorLogArea
{
    public class ErrorLogAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ErrorLogArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ErrorLogArea_default",
                "ErrorLogArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
