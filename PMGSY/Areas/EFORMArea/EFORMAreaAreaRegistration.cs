using System.Web.Mvc;

namespace PMGSY.Areas.EFORMArea
{
    public class EFORMAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EFORMArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EFORMArea_default",
                "EFORMArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
