using System.Web.Mvc;

namespace PMGSY.Areas.QMSSRSReports
{
    public class QMSSRSReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QMSSRSReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QMSSRSReports_default",
                "QMSSRSReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
