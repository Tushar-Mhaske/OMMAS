using System.Web.Mvc;

namespace PMGSY.Areas.WorkAwardedArea
{
    public class WorkAwardedAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkAwardedArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkAwardedArea_default",
                "WorkAwardedArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
