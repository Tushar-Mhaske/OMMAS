using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports
{
    public class OtherReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OtherReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OtherReports_default",
                "OtherReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
