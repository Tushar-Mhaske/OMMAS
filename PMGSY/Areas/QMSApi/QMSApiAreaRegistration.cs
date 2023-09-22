using System.Web.Mvc;

namespace PMGSY.Areas.QMSApi
{
    public class QMSApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QMSApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QMSApi_default",
                "QMSApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
