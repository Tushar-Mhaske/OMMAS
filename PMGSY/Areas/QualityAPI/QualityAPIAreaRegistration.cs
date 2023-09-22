using System.Web.Mvc;

namespace PMGSY.Areas.QualityAPI
{
    public class QualityAPIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QualityAPI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QualityAPI_default",
                "QualityAPI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
