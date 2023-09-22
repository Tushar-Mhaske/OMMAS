using System.Web.Mvc;

namespace PMGSY.Areas.FeedbackReport
{
    public class FeedbackReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FeedbackReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FeedbackReport_default",
                "FeedbackReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
