using System.Web.Mvc;

namespace PMGSY.Areas.DynamicData
{
    public class DynamicDataAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DynamicData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DynamicData_default",
                "DynamicData/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
