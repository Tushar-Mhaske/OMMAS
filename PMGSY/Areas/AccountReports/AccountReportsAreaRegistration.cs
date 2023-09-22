using System.Web.Mvc;

namespace PMGSY.Areas.AccountReports
{
    public class AccountReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AccountReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AccountReports_default",
                "AccountReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
