using System.Web.Mvc;

namespace PMGSY.Areas.ECBriefReport
{
    public class ECBriefReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ECBriefReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ECBriefReport_default",
                "ECBriefReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
